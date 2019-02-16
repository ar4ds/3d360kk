var vrotate : Vector3; 

//Rotate Left
function RotateLeft()  
{
	print("Rotate Left");
	transform.Rotate(Vector3.up * Time.deltaTime * 100, Space.World);
}  
 
//Rotate Right
function RotateRight()  
{
	print("Rotate Right");
	transform.Rotate(Vector3.down * Time.deltaTime * 100, Space.World);    
}  
 
//Rotate Up
function RotateUp()
{
	print("Rotate Up");
	transform.Rotate(Vector3.right * Time.deltaTime * 100, Space.World);    
}  
 
//Rotate Down
function RotateDown()
{
	print("Rotate Down");
	transform.Rotate(Vector3.left * Time.deltaTime * 100, Space.World);    
}