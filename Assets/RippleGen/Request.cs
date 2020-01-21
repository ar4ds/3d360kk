using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;
using RippleGen.Extends;
using System.Text.RegularExpressions;
using System.Threading;

namespace RippleGen
{
    public class Request : Operation.Entry
    {

        protected HttpWebRequest request;

        public HttpWebRequest WebRequest { get { return request; } }

        private HttpWebResponse response;
        public string responseFile;
        public Exception error;
        // 是否向服务端请求过期,如果没过期就不请求。
        public bool CheckExpire = true;
        public bool Override = false;
        private bool is304 = false;

        public bool Is304 { get { return is304; } }

        // 是否直接读取缓存区的文件
        public static bool StaticReadCache = false;
        public static bool StaticReadCacheWhenError = false;

        public bool ReadCache = false;
        public bool ReadCacheWhenError = false;
        // 缓存区文件名
        private string filename;

        // 是否断点续传
        public bool Breakpoints = false;
        public long totleLength = 0;
        public long downloadedLength = 0;

        private bool isCancel = false;
        private byte[] contentBuffer = null;

        public bool IsCancel { get { return isCancel; } }

        public TimeSpan Timeout = new TimeSpan(0);

        private System.Object lockThis = new System.Object();

        public Request(string url, Hashtable options = null, bool form = false, string method = "GET")
        {
            checkIndex();
            this.request = createRequest(url, options, method, form);
            string str = url;
            if (options != null)
            {
                str += createQuery(options);
            }
            ReadCache = StaticReadCache;
            ReadCacheWhenError = StaticReadCacheWhenError;
            filename = EncodeURL(str);
            Debug.Log("encode " + str + " to " + filename);
        }
        //<summary>
        // 创建新的请求.
        //</summary>
        public Request(HttpWebRequest request)
        {
            this.request = request;
        }

        // <summary>
        // 获得stream，使用后记得关闭
        // </summary>
        public Stream ResponseStream()
        {
            if (responseFile != null)
            {
                return File.OpenRead(responseFile);
            }
            else
            {
                return response.GetResponseStream();
            }
        }

        bool isTimeout = false;
        // Overwrites
        internal protected override void Start()
        {
            if (isCancel || !live)
                return;
            Debug.Log("start request " + request.RequestUri);
            if (ReadCache)
            {
                responseFile = RyanGlobalProps.cacheDir + filename;
                Debug.Log("file at " + responseFile);
                if (isOverInCache(filename) &&
                    File.Exists(responseFile))
                {
                    Debug.Log("read cached at " + responseFile);
                    threadComplete();
                    return;
                }
            }
            startRequest(request, (HttpWebResponse res) =>
            {
                response = res;
                threadComplete();
            });
            if (Timeout.Ticks > 0)
            {
                Action action = () =>
                {
                    Debug.Log("Start sleep");
                    Thread.Sleep(Timeout);
                    Debug.Log("End sleep");
                };
                action.BeginInvoke((ar) =>
                {
                    if (live)
                    {
                        isTimeout = true;
                        //                        if (currentstream != null) {
                        //                            currentstream.Close();
                        //                            currentstream = null;
                        //                        }
                        //                        if (netstream != null) {
                        //                            netstream.Close();
                        //                            netstream = null;
                        //                        }
                        this.error = new TimeoutException();
                        ((HttpWebRequest)request).Abort();
                        threadComplete();
                        action.EndInvoke(ar);
                    }
                }, null);
            }
        }


        Stream currentstream, netstream;

        public override void Cancel()
        {
            isCancel = true;
            //            if (currentstream != null) {
            //                currentstream.Close();
            //                currentstream = null;
            //            }
            //            if (netstream != null) {
            //                netstream.Close();
            //                netstream = null;
            //            }
            ((HttpWebRequest)request).Abort();
            if (this.error == null)
                this.error = new Exception("Thread is cancel.");
            base.Cancel();
        }


        internal protected override void Complete()
        {
            if (isCancel)
            {
                Hashtable options = new Hashtable();
                options.Add("status", "not-complete");
                Debug.LogError("Hello");
                cacheData(filename, options);
            }
            else if (live)
            {
                if (this.error != null)
                {
                    if (ReadCacheWhenError &&
                        isOverInCache(filename) &&
                        File.Exists(RyanGlobalProps.cacheDir + filename))
                    {
                        this.error = null;
                        Debug.Log("complete request " + request.RequestUri + " with cached");
                        responseFile = RyanGlobalProps.cacheDir + filename;
                        base.Complete();
                        return;
                    }
                    if (isTimeout)
                    {
                        Hashtable options = new Hashtable();
                        options.Add("status", "not-complete");
                        cacheData(filename, options);
                    }
                    Debug.Log("error when request " + request.RequestUri);
                    base.Complete();
                }
                else
                {
                    Debug.Log("complete request " + request.RequestUri);
                    Debug.Log("file name = " + responseFile);
                    Hashtable options = new Hashtable();
                    options.Add("status", "over");
                    options.Add("name", RyanGlobalProps.CurrentMuseumName);
                    options.Add("type", RyanGlobalProps.CurrentScene);
                    options.Add("size", new FileInfo(responseFile).Length.ToString());
                    if (response != null && response.Headers["Last-Modified"] != null)
                        options.Add("modified", response.Headers["Last-Modified"]);
                    cacheData(filename, options);
                    base.Complete();
                }
            }
        }

        private void startRequest(HttpWebRequest request, Action<HttpWebResponse> responseAction)
        {
            Action wrapperAction = () =>
            {
                bool breakpoint = Breakpoints;
                if (isCancel)
                    return;
                // 如果需要检测304
                if (CheckExpire &&
                    exitsData(filename) &&
                    File.Exists(RyanGlobalProps.cacheDir + filename) &&
                    indexes[filename]["modified"] != null)
                {
                    request.IfModifiedSince = Convert.ToDateTime(indexes[filename]["modified"].Value);

                }
                // 是否断点续传
                int readed = 0;
                if (breakpoint && File.Exists(RyanGlobalProps.cacheDir + filename))
                {
                    Stream stream = File.OpenRead(RyanGlobalProps.cacheDir + filename);
                    readed = (int)stream.Length;
                    stream.Close();
                    request.AddRange((int)readed);
                }
                else
                {
                    breakpoint = false;
                }

                if (contentBuffer != null)
                {
                    try
                    {
                        Stream stream = request.GetRequestStream();
                        stream.Write(contentBuffer, 0, contentBuffer.Length);
                        stream.Close();
                    }
                    catch (WebException e)
                    {
                        Debug.Log("Can not open stream");
                        this.error = e;
                        responseAction(null);
                        return;
                    }
                }

                try
                {

                    request.BeginGetResponse(new AsyncCallback((iar) =>
                    {
                        if (isCancel)
                            return;
                        HttpWebResponse response = null;
                        try
                        {
                            response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                        }
                        catch (WebException e)
                        {
                            if (e.Response != null &&
                                (e.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotModified &&
                                exitsData(filename) &&
                                File.Exists(RyanGlobalProps.cacheDir + filename))
                            {
                                responseFile = RyanGlobalProps.cacheDir + filename;
                                response = null;
                                is304 = true;
                                Debug.Log("is 304");
                            }
                            else
                            {
                                Debug.Log("Error point 2");
                                this.error = e;
                                Debug.LogError(this + " -> startRequest -> " + e.ToString());
                            }
                        }

                        if (response != null)
                        {
                            bool isBreakpoint = breakpoint && response.Headers["Accept-Ranges"] == "bytes";
                            try
                            {
                                // 写入缓存文件
                                totleLength = response.ContentLength + readed;
                                netstream = response.GetResponseStream();

                                int size = 4096;
                                if (totleLength > 1024 * 1024 * 10)
                                {
                                    size = 1024 * 1024 * 5;
                                }
                                byte[] buffer = new byte[size];
                                // 判断是否断点续传, 是就追加
                                if (!isBreakpoint)
                                {
                                    File.Delete(RyanGlobalProps.cacheDir + filename);
                                }
                                currentstream = File.Open(RyanGlobalProps.cacheDir + filename, FileMode.OpenOrCreate);
                                if (isBreakpoint)
                                {
                                    currentstream.Seek(0, SeekOrigin.End);
                                }
                                int readSize = netstream.Read(buffer, 0, (int)buffer.Length);
                                while (readSize > 0)
                                {
                                    if (isCancel || isTimeout)
                                        break;
                                    downloadedLength += readSize;
                                    threadProgress((readed + downloadedLength) / (float)totleLength);
                                    currentstream.Write(buffer, 0, readSize);
                                    readSize = netstream.Read(buffer, 0, (int)buffer.Length);
                                }
                                currentstream.Close();
                                netstream.Close();
                                currentstream = null;
                                netstream = null;
                                Hashtable options = new Hashtable();
                                options.Add("status", "over");
                                responseFile = RyanGlobalProps.cacheDir + filename;
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Error point 1");
                                //                            if (isBreakpoint) {
                                //                                Hashtable options = new Hashtable();
                                //                                options.Add("status", "breakpoint");
                                //                                options.Add("totle", totleLength.ToString());
                                //                                options.Add("point", downloadedLength.ToString());
                                //                                cacheData(filename, options);
                                //                            }
                                if (!isTimeout)
                                    this.error = e;
                            }
                        }
                        responseAction(response);
                    }), request);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            };
            wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
            {
                var action = (Action)iar.AsyncState;
                action.EndInvoke(iar);
            }), wrapperAction);
        }

        private HttpWebRequest createRequest(string url, Hashtable options, string method, bool form)
        {
            string contentType = "application/x-www-form-urlencoded";
            if (options != null && options.Count > 0)
            {
                if (method == "GET")
                {
                    url = url + "?" + createQuery(options);
                }
                else
                {
                    WWWForm wwwForm = new WWWForm();
                    IDictionaryEnumerator enumerator = options.GetEnumerator();
                    if (form)
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Key != null && enumerator.Value != null)
                                wwwForm.AddBinaryData(enumerator.Key.ToString(), System.Text.Encoding.Default.GetBytes(enumerator.Value.ToString()));
                        }
                        contentType = "multipart/form-data";
                    }
                    else
                    {
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Key != null && enumerator.Value != null)
                                wwwForm.AddField(enumerator.Key.ToString(), enumerator.Value.ToString());
                        }
                    }
                    contentBuffer = wwwForm.data;
                }
            }

            HttpWebRequest request = (System.Net.WebRequest.Create(url)) as HttpWebRequest;
            request.Method = method;
            request.Timeout = 10000;
            if (contentBuffer != null)
                request.ContentLength = contentBuffer.Length;
            request.ContentType = contentType;

            return request;
        }

        private static string createQuery(Hashtable options)
        {
            IDictionaryEnumerator enumerator = options.GetEnumerator();
            string query = "";
            bool first = true;
            while (enumerator.MoveNext())
            {
                if (first)
                    query += "?";
                else
                    query += "&";
                first = false;
                query += WWW.EscapeURL(enumerator.Key.ToString()) + "=" + WWW.EscapeURL(enumerator.Value.ToString());
            }
            return query;
        }

        private void invokeDeletetes<T>(List<Action<T>> delegates, T target)
        {
            foreach (Action<T> d in delegates)
            {
                d(target);
            }
        }

        public static string urlWithQuery(string url, Hashtable options)
        {
            return url + createQuery(options);
        }

        #region Ryan

        public static bool Exist(string url)
        {
            return File.Exists(CachedPath(url));
        }

        public static void Delete(string url)
        {
            File.Delete(CachedPath(url));
        }
        public static void DeleteLocal(string filename)
        {
            checkIndex();
            JSONNode jn = indexes;
            jn.Remove(filename);
            _writeIndex(jn);
            File.Delete(filename);
        }

        #endregion

        static Regex exReg = new Regex("\\.\\w+$");

        public static string EncodeURL(string url)
        {
            string extend = "";
            if (exReg.IsMatch(url))
            {
                extend = exReg.Match(url).Value;
            }

            return RippleGen.Utils.Coder.EncodeMd5(url) + extend;
        }

        public static bool IsCached(string url)
        {
            string filename = EncodeURL(url);
            Debug.LogError(filename + " >>> " + url);
            checkIndex();
            return isOverInCache(filename) && File.Exists(RyanGlobalProps.cacheDir + filename);
        }

        public static string CachedPath(string url)
        {
            string filename = EncodeURL(url);
            checkIndex();
            if (isOverInCache(filename) && File.Exists(RyanGlobalProps.cacheDir + filename))
            {
                return RyanGlobalProps.cacheDir + filename;
            }
            return null;
        }

        static JSONNode indexes;

        static void checkIndex()
        {
            if (!Directory.Exists(RyanGlobalProps.cacheDir))
            {
                Directory.CreateDirectory(RyanGlobalProps.cacheDir);
            }
            if (indexes == null)
            {
                if (File.Exists(RyanGlobalProps.indexPath))
                {
                    indexes = _readIndex();
                }
                else
                {
                    indexes = JSON.Parse("{}");
                    _writeIndex(indexes);
                }
            }
        }

        static long breakpointReaded(string name)
        {
            if (indexes[name] != null && indexes[name]["status"].Value == "breakpoint" && indexes[name]["point"] != null)
                return indexes[name]["point"].AsInt;
            return 0;
        }

        static bool isOverInCache(string name)
        {
            return indexes[name] != null && indexes[name]["status"].Value == "over";
        }

        static bool exitsData(string name)
        {
            return indexes[name] != null;
        }

        static void cacheData(string name, Hashtable options)
        {
            JSONNode node = indexes[name];
            if (node != null)
            {
                indexes.Remove(name);
            }
            node = JSON.Parse("{}");
            IDictionaryEnumerator enu = options.GetEnumerator();
            while (enu.MoveNext())
            {
                if(!string.IsNullOrEmpty(enu.Value as string)){
                    node.Add(enu.Key as string, new JSONString(enu.Value as string));
                }
            }
            if (!node.IsNull)
            {
                indexes.Add(name, node);
            }
            _writeIndex(indexes);
        }

        static JSONNode _readIndex()
        {
            return JSON.Parse(File.ReadAllText(RyanGlobalProps.indexPath));
        }

        static void _writeIndex(JSONNode node)
        {
            File.WriteAllText(RyanGlobalProps.indexPath, node.ToString());
        }

    }
}