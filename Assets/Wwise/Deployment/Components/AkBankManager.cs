#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;

public class AkBankHandle
{
    int m_RefCount = 0;
    public uint m_BankID;

    public string relativeBasePath;
    public string bankName;
    public bool decodeBank;
    public bool saveDecodedBank;

    public AkCallbackManager.BankCallback bankCallback;

    public AkBankHandle(string name, bool decode, bool save)
    {
        bankName = name;
        bankCallback = null;
        decodeBank = decode;
		saveDecodedBank = save;

        // Verify if the bank has already been decoded
        if( decodeBank )
		{
            string DecodedBankPath = System.IO.Path.Combine(AkInitializer.GetDecodedBankFullPath(), bankName + ".bnk");
            string EncodedBankPath = System.IO.Path.Combine(AkBasePathGetter.GetValidBasePath(), bankName + ".bnk");
			if ( System.IO.File.Exists (DecodedBankPath) )
			{
				try
				{
					if (System.IO.File.GetLastWriteTime(DecodedBankPath) > System.IO.File.GetLastWriteTime(EncodedBankPath))
					{
						relativeBasePath = AkInitializer.GetDecodedBankFolder();
						decodeBank = false;
					}
				}
				catch
				{
					// Assume the decoded bank exists, but is not accessible. Re-decode it anyway, so we do nothing.
				}
			}
		}
    }

    public int RefCount
    {
        get
        {
            return m_RefCount;
        }
    }

	/// Loads a bank. This version blocks until the bank is loaded. See AK::SoundEngine::LoadBank for more information.
	public void LoadBank()
	{
		if (m_RefCount == 0)
		{
			AKRESULT res = AKRESULT.AK_Fail;
			
			// There might be a case where we were asked to unload the SoundBank, but then asked immediately after to load that bank.
			// If that happens, there will be a short amount of time where the ref count will be 0, but the bank will still be in memory.
			// In that case, we do not want to unload the bank, so we have to remove it from the list of pending bank unloads.
			if(AkBankManager.BanksToUnload.Contains(this))
			{
				AkBankManager.BanksToUnload.Remove(this);
				IncRef();
				return;
			}

            if( decodeBank == false )
			{
                string basePathToSet = null;

                if (!string.IsNullOrEmpty(relativeBasePath))
                {
                    basePathToSet = AkBasePathGetter.GetValidBasePath();
                    if (string.IsNullOrEmpty(basePathToSet))
                    {
                        Debug.LogWarning("WwiseUnity: Bank " + bankName + " failed to load (could not obtain base path to set).");
                        return;
                    }

                    res = AkSoundEngine.SetBasePath(System.IO.Path.Combine(basePathToSet, relativeBasePath));
                }
                else
                {
                    res = AKRESULT.AK_Success;
                }

                if (res == AKRESULT.AK_Success)
                {
				    res = AkSoundEngine.LoadBank(bankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);
	                
					if (!string.IsNullOrEmpty(basePathToSet))
	                {
	                    AkSoundEngine.SetBasePath(basePathToSet);
	                }
				}
			}
			else
			{
				res = AkSoundEngine.LoadAndDecodeBank(bankName, saveDecodedBank, out m_BankID);
			}

            if (res != AKRESULT.AK_Success)
			{
				Debug.LogWarning("WwiseUnity: Bank " + bankName + " failed to load (" + res.ToString() + ")");
			}
		}
		IncRef();  
	}

	/// Loads a bank.  This version returns right away and loads in background. See AK::SoundEngine::LoadBank for more information
	public void LoadBankAsync(AkCallbackManager.BankCallback callback = null)
	{
		if (m_RefCount == 0)
		{
			// There might be a case where we were asked to unload the SoundBank, but then asked immediately after to load that bank.
			// If that happens, there will be a short amount of time where the ref count will be 0, but the bank will still be in memory.
			// In that case, we do not want to unload the bank, so we have to remove it from the list of pending bank unloads.
			if(AkBankManager.BanksToUnload.Contains(this))
			{
				AkBankManager.BanksToUnload.Remove(this);
				IncRef();
				return;
			}
			
			bankCallback = callback;
			AkSoundEngine.LoadBank(bankName, AkBankManager.GlobalBankCallback, this, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);
		}
		IncRef();
	}

    public void IncRef()
    {       
        m_RefCount++;
    }

    public void DecRef()
    {
        m_RefCount--;
        if (m_RefCount == 0)
        {
            AkBankManager.BanksToUnload.Add(this);
        }
    }
}

/// @brief Maintains the list of loaded SoundBanks loaded. This is currently used only with AkAmbient objects.
public static class AkBankManager
{	
    static Dictionary<string, AkBankHandle> m_BankHandles = new Dictionary<string, AkBankHandle>();
    static public List<AkBankHandle> BanksToUnload = new List<AkBankHandle>();
    
	static public void DoUnloadBanks()
	{
		foreach(AkBankHandle bank in BanksToUnload)
		{
            if( bank.decodeBank == true && bank.saveDecodedBank == false )
            {
				AkSoundEngine.PrepareBank(PreparationType.Preparation_Unload, bank.m_BankID);
            }
            else
            {
				AkSoundEngine.UnloadBank(bank.m_BankID, IntPtr.Zero, null, null);
			}
		}
		
		BanksToUnload.Clear();
	}
	
	static public void Reset()
	{
		m_BankHandles.Clear ();
		BanksToUnload.Clear ();
	}
	
	static public void GlobalBankCallback(uint in_bankID, IntPtr in_pInMemoryBankPtr, AKRESULT in_eLoadResult, uint in_memPoolId, object in_Cookie)
    {
		m_Mutex.WaitOne();	
		AkBankHandle handle = (AkBankHandle)in_Cookie ;
		AkCallbackManager.BankCallback cb = handle.bankCallback;
		if (in_eLoadResult != AKRESULT.AK_Success)
		{			
			Debug.LogWarning("WwiseUnity: Bank " + handle.bankName + " failed to load (" + in_eLoadResult.ToString() + ")");
			m_BankHandles.Remove(handle.bankName);
		}
		m_Mutex.ReleaseMutex();

		if (cb != null)
			cb(in_bankID, in_pInMemoryBankPtr, in_eLoadResult, in_memPoolId, null);
    }

	/// Loads a SoundBank. This version blocks until the bank is loaded. See AK::SoundEngine::LoadBank for more information.
    public static void LoadBank(string name, bool decodeBank, bool saveDecodedBank)
    {
		m_Mutex.WaitOne();
		AkBankHandle handle = null;
		if (!m_BankHandles.TryGetValue(name, out handle))
		{
			handle = new AkBankHandle(name, decodeBank, saveDecodedBank);
			m_BankHandles.Add(name, handle);			
			m_Mutex.ReleaseMutex();
			handle.LoadBank();  		
		}
		else
		{
			// Bank already loaded, increment its ref count.
			handle.IncRef();
			m_Mutex.ReleaseMutex();
		}
    }
	
	/// Loads a SoundBank. This version returns right away and loads in background. See AK::SoundEngine::LoadBank for more information.
	public static void LoadBankAsync(string name, AkCallbackManager.BankCallback callback = null)
	{
		m_Mutex.WaitOne();
		AkBankHandle handle = null;
		if (!m_BankHandles.TryGetValue(name, out handle))
		{
			handle = new AkBankHandle(name, false, false);
			m_BankHandles.Add(name, handle);			
			m_Mutex.ReleaseMutex();
			handle.LoadBankAsync(callback);  		
		}
		else
		{
			// Bank already loaded, increment its ref count.
			handle.IncRef();
			m_Mutex.ReleaseMutex();
		}
	}

	/// Unloads a SoundBank. See AK::SoundEngine::UnloadBank for more information.
    public static void UnloadBank(string name)
    {
		m_Mutex.WaitOne();
		AkBankHandle handle = null;
		if (m_BankHandles.TryGetValue(name, out handle))
		{
			handle.DecRef();
			if (handle.RefCount == 0)
				m_BankHandles.Remove(name);
		}
		m_Mutex.ReleaseMutex();
    }
	
	static System.Threading.Mutex m_Mutex = new System.Threading.Mutex();
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.