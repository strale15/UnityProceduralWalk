using System;

public class Class1
{
	public Class1()
	{
        private void DoLeftSideLerp()
        {
            //...

            //Slerp stopala
            leftFoot.rotation = 
                Quaternion.Slerp(leftFootStartRotLerp, leftFootGoalRotation, lerp);

            //...
        }
	}
}
