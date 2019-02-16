#import "UnityAppController.h"
#import <MediaPlayer/MediaPlayer.h>
extern "C"
{
    void _playMyVideo(const char *url){
        NSURL* movieURL=[NSURL URLWithString:[NSString stringWithCString:url encoding:NSUTF8StringEncoding]];
        MPMoviePlayerController *player=[[MPMoviePlayerController alloc]initWithContentURL:movieURL];
        [player prepareToPlay];
        [UnityGetMainWindow().window addSubview:player.view];
        player.shouldAutoplay=YES;
        [player setControlStyle:MPMovieControlStyleDefault];
        [player setFullscreen:YES];
        [player.view setFrame:[UnityGetMainWindow() window].bounds];
    }
    
    void _collectMuseum(const char *url){
        
    }
    
    int _popHomepage(){
        return 0;
    }
    
    int _backHomeFromAR(){
        return 0;
    }
    
    int _backHomeFromVR(){
        return 0;
    }
}

