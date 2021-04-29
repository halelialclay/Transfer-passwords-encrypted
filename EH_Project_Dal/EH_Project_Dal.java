package EH_Project_Dal;

import com.intel.crypto.CryptoException;
import com.intel.crypto.Random;
import com.intel.crypto.RsaAlg;
import com.intel.crypto.SymmetricBlockCipherAlg;
import com.intel.util.*;

//
// Implementation of DAL Trusted Application: EH_Project_Dal 
//
// **************************************************************************************************
// NOTE:  This default Trusted Application implementation is intended for DAL API Level 7 and above
// **************************************************************************************************

public class EH_Project_Dal extends IntelApplet {

	/**
	 * This method will be called by the VM when a new session is opened to the Trusted Application 
	 * and this Trusted Application instance is being created to handle the new session.
	 * This method cannot provide response data and therefore calling
	 * setResponse or setResponseCode methods from it will throw a NullPointerException.
	 * 
	 * @param	request	the input data sent to the Trusted Application during session creation
	 * 
	 * @return	APPLET_SUCCESS if the operation was processed successfully, 
	 * 		any other error status code otherwise (note that all error codes will be
	 * 		treated similarly by the VM by sending "cancel" error code to the SW application).
	 */
	public int onInit(byte[] request) {
		DebugPrint.printString("Hello, DAL!");
		return APPLET_SUCCESS;
	}
	
	
	int file=0;
	public SymmetricBlockCipherAlg myAlg128=SymmetricBlockCipherAlg.create(SymmetricBlockCipherAlg.ALG_TYPE_AES_ECB);
	public RsaAlg MyRsa=RsaAlg.create();
	
	/**
	 * This method will be called by the VM to handle a command sent to this
	 * Trusted Application instance.
	 * 
	 * @param	commandId	the command ID (Trusted Application specific) 
	 * @param	request		the input data for this command 
	 * @return	the return value should not be used by the applet
	 */
	public int invokeCommand(int commandId, byte[] request) {
		
		int errornum = 0;
		try {
			DebugPrint.printString("Received command Id: " + commandId + ".");
			if(request != null)
			{
				DebugPrint.printString("Received buffer:");
				DebugPrint.printBuffer(request);
			}
			
	errornum = 1;
		switch (commandId)  {
		
			case 3:{
				
				
				boolean x= createSymmetricKey();
				if(x==true)
				DebugPrint.printString("true");
				
			    setResponse(request, 0, request.length);
							 
			}
			case 5:{
				
				
				      

						MyRsa.setHashAlg(RsaAlg.HASH_TYPE_SHA1);
						errornum = 10;
						
						MyRsa.setPaddingScheme(RsaAlg.PAD_TYPE_PKCS1);
						
						
					    errornum = 20;
					    byte [] e =new byte[3];
				    	byte [] mod=new byte[128];
						     
						
				    	DebugPrint.printString("public buffer:");
						int j=0,k=0;
						for(int i=0;i<request.length ;i++)
						{
							if(i<128)
							{
								mod[j]=request[i];
								j++;
							}
							else { 
								e[k]=request[i];
								k++;
							}
						}
						
						
						
					   Short index1 = 0 ;
					   Short index2 = 16 ;
						
                       byte [] encryptSymmetricKey =new byte [128];
						
						errornum = 70;
						MyRsa.setKey(mod,index1,(short)mod.length,
								e,index1,(short)e.length);
						errornum = 80;
						
			
						DebugPrint.printString("public buffer:");						
						DebugPrint.printBuffer(mod);
						DebugPrint.printString("hhhhhh");
						DebugPrint.printBuffer(e);
					
								
								
					int size=FlashStorage.getFlashDataSize(file);
					DebugPrint.printInt(size);
					byte[] Symmetric = new byte[size];
					FlashStorage.readFlashData(file,Symmetric,0);
    				 errornum = 30;
					 
					 DebugPrint.printBuffer(Symmetric);
			
				    	
					 myAlg128.setKey(Symmetric,index1,index2);
				   	 errornum = 40;
			
				   	myAlg128.getKey(Symmetric,(short)0);
				   	errornum = 50;
		
						
					MyRsa.encryptComplete(Symmetric, index1,
							(short)Symmetric.length , encryptSymmetricKey,(short)0);
					errornum = 90;
				    setResponse( encryptSymmetricKey, 0, encryptSymmetricKey.length);

						
			}
			
			
			
			
			
			
			case 7:{
				
			   int size=FlashStorage.getFlashDataSize(file);
			   short index1=0;
			   short index2=(short)size;
			   
			   byte[] Symmetric = new byte[size];
			   String  a;
				
				FlashStorage.readFlashData(file,Symmetric,0);
				DebugPrint.printInt(size);
			    DebugPrint.printBuffer(Symmetric);
		

				 myAlg128.setKey(Symmetric, index1, index2);

			   byte [] decryptdata =new byte [548];
			   
			   myAlg128.decryptComplete(request, index1,(short) request.length, decryptdata, index1);

			   
			   DebugPrint.printBuffer(decryptdata);
			   
			    setResponse( decryptdata, 0, decryptdata.length);
				
			}
		
			
			
			
		}							
		
		}
		
		
		
		
		
		catch(FileNotFoundException e) {
			DebugPrint.printString("errornum = " + errornum+"1");
			DebugPrint.printString(e.toString());
		}
		catch(IllegalParameterException e) {
			DebugPrint.printString("errornum = " + errornum+"2");
		}
		catch(UtilSecurityException e) {
			DebugPrint.printString("errornum = " + errornum+"4");
		}
		catch(UtilException e) {
			DebugPrint.printString("errornum = " + errornum+"4");
		}
		
		catch(Exception e) {
			DebugPrint.printString("errornum = " + errornum+"3");
		}
		
		
		
		setResponseCode(commandId);
		return APPLET_SUCCESS;
	}
	
public boolean createSymmetricKey() {
	
		int errornum = 0;
		try {
		Short short1 = 0 ;
		Short short2 = 16 ;
		
		byte[] Symmetric = new byte[16];   
		Random.getRandomBytes(Symmetric, short1, short2);
	
		 
		myAlg128.setKey(Symmetric, short1, short2);
		myAlg128.getKey(Symmetric,(short)0);
	    DebugPrint.printBuffer(Symmetric);
		
	    errornum = 2;
		DebugPrint.printInt(Symmetric.length);
		
		errornum = 3;
	    FlashStorage.writeFlashData(file,Symmetric,0,Symmetric.length);
	 
	    errornum = 4;
	  //  return true;
	    
		}
	
	catch(FileNotFoundException e) {
		DebugPrint.printString("errornum = " + errornum+"1");
		DebugPrint.printString(e.toString());
	}
	catch(IllegalParameterException e) {
		DebugPrint.printString("errornum = " + errornum+"2");
	}
	catch(UtilSecurityException e) {
		DebugPrint.printString("errornum = " + errornum+"4");
	}
	catch(UtilException e) {
		DebugPrint.printString("errornum = " + errornum+"4");
	}
	
	catch(Exception e) {
		DebugPrint.printString("errornum = " + errornum+"3");
	}
 
	
    return true;
	
}


static byte[] reverse(byte a[], int n) 
{ 
	byte[] b = new byte[n]; 
    int j = n; 
    for (int i = 0; i < n; i++) { 
        b[j - 1] = a[i]; 
        j = j - 1; 
    } 
    
    return b;
}

	/**
	 * This method will be called by the VM when the session being handled by
	 * this Trusted Application instance is being closed 
	 * and this Trusted Application instance is about to be removed.
	 * This method cannot provide response data and therefore
	 * calling setResponse or setResponseCode methods from it will throw a NullPointerException.
	 * 
	 * @return APPLET_SUCCESS code (the status code is not used by the VM).
	 */
	public int onClose() {
		DebugPrint.printString("Goodbye, DAL!");
		return APPLET_SUCCESS;
	}
}
