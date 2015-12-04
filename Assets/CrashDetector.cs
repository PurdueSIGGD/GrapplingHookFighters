using System;
using System.Threading;
//Author: Glurth
//Puropose:
//  Sometimes using the Pause option on the debugger does not work.  Breakpoints however DO seem to work.
//  This class can be used to detect infitine-loops or similar problems, and terminate the program.
//Usage:
//  At various places throughout your code call CrashDetector.SetExePoint(string), passing a unique string value each time.
//    The first time you call SetExePoint, it will start monitoring
//      You should call SetExePoint with a different value at least once in a while
//    If it does NOT get a diferent value after a particular counter threshold is reached, It will terminate the main thread! (unity.exe)
//
//  If Using the debugger:  you can add a breakpoint to the /*put a breakpoint here*/ line, in the code below.
//      Check the value of lastExecPoint in the debugger when it hits the breakpoint, to see where your last call to SetExePoint was.


static class CrashDetector
{
	static string execPoint;
	static string lastExecPoint;
	static long lastExecPointTime;
	static long crashTimeTreshold=100000;
	static Thread monitoringThread;
	static string dontCountPoint="No Exec points set yet. 10923$$#!!";
	
	static CrashDetector()
	{
		execPoint=dontCountPoint;
		lastExecPoint=dontCountPoint;
		monitoringThread= new Thread(CrashDetectorThread);
		monitoringThread.Start(Thread.CurrentThread);
	}
	
	public static void SetExePoint(string exe_point){execPoint=exe_point;}
	public static void SetCrashTimeTreshold(long newThreshold){crashTimeTreshold=newThreshold;}
	
	static void CrashDetectorThread(object calling_thread)  // use Thread.CurrentThread when calling function, to set this param
	{
		Thread mainthread=(Thread) calling_thread;
		long crash_counter=0;
		while(true)// infite-loop detector loop: I love ironic code
		{
			if(dontCountPoint!=lastExecPoint)// have ANY execution points been set yet?
				crash_counter++;
			
			//WARNING:  this code does not yet include a threadsafe read of the execPoint or crashTimeTreshold variables used below
			if(execPoint!=lastExecPoint)// have we reached a new execution point- if so, reset count
			{
				lastExecPoint=execPoint;
				crash_counter=0;
			}
			if(crash_counter>crashTimeTreshold) //if too long has passed without change of execution point
			{
				/*put a breakpoint here*/
				//Debug.Log("aborting main thread:" + lastExecPoint);
				mainthread.Abort();  // terminates main thread (e.g unity.exe: this will close unity)
				mainthread.Join ();  // Waits untill that thread is closed
				return; // closes this thread cleanly.
			}
			Thread.Sleep(0); // counter not execeeded: relinquish remainder of this thread's processing timeslice.
		}
	}
}