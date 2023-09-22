using System;

public class Class1
{
	public Class1()
	{
        private void DoLeftSideLerp()
        {
            //...

            //Slerp kicme
            lowerSpine.rotation = 
                Quaternion.Slerp(lowerSpineCurrentRot, lowerSpineGoalRot, lerp);
            middleSpine.rotation = 
                Quaternion.Slerp(middleSpineCurrentRot, middleSpineGoalRot, lerp);

            Vector3 rot = upperSpine.eulerAngles;
            upperSpine.rotation = 
                Quaternion.Slerp(upperSpineCurrentRot, upperSpineGoalRot, lerp);
            upperSpine.rotation = 
                Quaternion.Euler(rot.x, upperSpine.eulerAngles.y, rot.z);

            //...
        }
	}
}
