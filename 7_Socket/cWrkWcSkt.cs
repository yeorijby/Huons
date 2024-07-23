using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data;
namespace WCS_TASK_SC
{
    class cWrkWcSkt
    {
        // @@@.����
        public string mDVC_NO = "001";                      	// @.PLC ��ȣ 
        public string mStatComm  = "D" ;                       	// @.PLC�� ����ϴ� ������ �������[C:����, T:�õ�, D:�񿬰�]
        public string mStatOp  = "W" ;                         	// @.PLC�� ����ϴ� ������ ���ۻ���[N:����, W:���, E:����]

        public cDbUse mDbWrk = new cDbUse("Multi", false ) ;            	// @.DB Class[Work]
        public cDbUse mDbLog = new cDbUse("Multi", false );             	// @.DB Class[Log]
        private DataTable mDtReqIF = new DataTable();                  	// @.DataTable[��û]
        public string mRcvTgm   = "";                           // @.����Tgm
        public string mMsg ="";                                   // @.�޼���

        // @@.���Ͽ��� ���Ǵ� ��������
        public Socket  mSkt;                                   // @.���ϰ�ü����
        //public bool  mSktConnected= false ;                         // @.�������ӿ���
        public int mReSndCnt  = 3;                         // @.������Ƚ��
        //public byte [] mByRxBuff;                               // @.���Ź���
        public byte[] mByTxBuff;                               // @.���۹���
        //public byte[] mReqTxBuff;                              // @.���۵����͹���

        // @@.WGT ��� �������
        public const int  MC_SND_TGM_BYTE_LEN   = 6;         // @.����Tgm ����[Byte]
        public const int MC_RCV_TGM_BYTE_LEN   = 22;        // @.������Tgm ����[Byte]

        // @@.���������� �ƴҰ�� ������ ������� �õ��Ѵ�. 
        public int mCnttryWgt  = 0   ;                      // @.�����õ� ī��Ʈ
        public double  mMaxtryWgt  = 0    ;                     // @.�Ҿ����߷� �ִ밪

        // @@@.������
        public cWrkWcSkt(string pDVC_NO)
        {
            mDVC_NO = pDVC_NO;     // @.System ID 
            if(cCmLib.GfDBLogIn (ref this.mDbWrk.mCnMain, ref mMsg) == true) this.mDbWrk.Init(); // @.DB Connetction
            if(cCmLib.GfDBLogIn (ref this.mDbLog.mCnMain, ref mMsg) == true) this.mDbLog.Init(); // @.DB Connetction
        }

        // @@@.Db Connection Close
        public void DBLogOut(){
            if(this.mDbWrk.mCnMain.State == ConnectionState.Open ) this.mDbWrk.mCnMain.Close(); this.mDbWrk.DbConnted = false;
            if(this.mDbLog.mCnMain.State == ConnectionState.Open ) this.mDbLog.mCnMain.Close(); this.mDbLog.DbConnted = false;
        }

        // @@@.Db Connection Open
        public void DBLogIn(){
            this.DBLogOut();
            if (cCmLib.GfDBLogIn(ref this.mDbWrk.mCnMain, ref mMsg) == true) this.mDbWrk.Init();
            if (cCmLib.GfDBLogIn(ref this.mDbLog.mCnMain, ref mMsg) == true) this.mDbLog.Init();
        }

        #region @@@.�Ӽ�[IsSktConnect]
        private bool  mSktConnected = false;      // @.IsSktConnect
        public bool IsSktConnect 
        {
          get {
            return mSktConnected;
          }
        }
        #endregion

        // @@@.SktDisconnect
        public void  SktDisconnect() 
        {
            if( IsSktConnect==false)
            {
                this.SktClose(ref mMsg);
                this.mSktConnected = false;
                this.mStatOp = "E";
            }
        }
 
        // @@@.SktConnect
        public bool SktConnect(string pIPAdress ,
                                   int pPort,
                          ref string  pMsg ) {
             System.Net.IPEndPoint rEP;
             System.Net.IPAddress rIP;

            try
            {
                pMsg = "";

                rIP = System.Net.IPAddress.Parse(pIPAdress);
                rEP = new System.Net.IPEndPoint(rIP, pPort);
                mSkt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mSkt.Connect(rEP);

                if( mSkt.Connected == false ) throw new Exception();

                mStatOp = "W";
                this.mSktConnected = true;
                pMsg = "Socket Connected";
                return  true;
            }
            catch(SocketException se) 
            {
                pMsg = se.Message;
            }
            catch(Exception ex)
            { 
                pMsg = ex.Message;
            }
            mStatOp = "E";
            mSktConnected = false;
            pMsg = "Fail Socket Connect::" + pMsg;
            return  false;
        }

        // @@@.���� �ݱ�
        private bool SktClose(ref string  pMsg)
        {
            try
            {
                pMsg = "";
                if(mSkt!=null){
                    if( mSkt.Connected==true){
                        mSkt.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    }
                    mSkt.Close();
                    mSkt = null; // @.���α׷������� ��������� ���� ��ü ��뿩�� �Ǵ�
                }

                pMsg = "Socket Close";
                return true;
            }
            catch(Exception ex) 
            {
                pMsg = ex.Message;
            }
            pMsg = "Fail Socket Close" + pMsg;
            return false;
        }

    // @@@.W/C�� WGT�跮 ��ûTgm ����
    public bool SndReqWC(ref string  pMsg) 
    {
        mByTxBuff = new byte[MC_SND_TGM_BYTE_LEN];
        string  sSnd = "";

        try
        {
            pMsg = "";
            sSnd = this.mDVC_NO.Substring(this.mDVC_NO.Length - 2, 2) + "RW" + cDefApp.CRLF;
            //sSnd = "01" + "RW" + CRLF
            mByTxBuff = System.Text.Encoding.Default.GetBytes(sSnd);

            //mByTxBuff(0) = CByte(Hex(Asc("0")))
            //mByTxBuff(1) = CByte(Hex(Asc("2")))
            //mByTxBuff(2) = CByte(Hex(Asc("R")))
            //mByTxBuff(3) = CByte(Hex(Asc("W")))
            //mByTxBuff(4) = &HD
            //mByTxBuff(5) = &HA

            if (Snd(mByTxBuff, mByTxBuff.Length, SocketFlags.None, false, cDefApp.GM_COMM_SND_TIME_OUT, ref pMsg) == false) throw new Exception();

            pMsg = "W/C�� �߷����� ��ûTgm ���� ����";
            return true;
        }
        catch(Exception ex) 
        {
            pMsg = ex.Message;
        }    
            pMsg = "W/C�� �߷����� ��ûTgm ���� ����::" + pMsg;
            return false;
    }

    // @@@.W/C�� �۽��� �߷� Tgm����
    public bool RcvAckWgt(ref byte[] pRxBuff,
                           int pReadByteLen, 
                   ref string  pMsg) 
    {
        pRxBuff = new byte[pReadByteLen];
        //Dim sRcvTgm As String = ""

        try
    {
            pMsg = "";

            // @.���ŵ�Tgm �� �о�´�.[���н� ��������]
            if (Rcv(ref pRxBuff, pRxBuff.Length, SocketFlags.None, cDefApp.GM_COMM_RCV_TIME_OUT, ref pMsg) == false) throw new Exception(); 

            pMsg = "WGT���� �۽��� �߷�Tgm ���� ����";
            return true;
    }
        catch(Exception ex) 
    {
            pMsg = ex.Message;
            }
        pMsg = "WGT���� �۽��� �߷�Tgm ���� ����::" + pMsg;
        return false;
    }

    // @@@.WGT�� Tgm�� ����ó���Ѵ�.
    private bool  Snd(byte[] pTxBuff,
                         int pSize, 
                SocketFlags  pFlag , 
                bool  pReSnd ,
                int pTmOutMil,
                ref string  pMsg )
    {
        int nSnd=0;

        try
        {
            pMsg = "";
            while (true)
            {
                if (nSnd > mReSndCnt)
                {
                    pMsg = "Send Count Over " + mReSndCnt.ToString() + " Times";
                    break;
                }

                nSnd += 1;

                // @.�� ���� ���� �� ��
                if( mSkt.Send(pTxBuff, pSize, pFlag) < 1)
                {
                    if( pReSnd == true )
                    {
                        Thread.Sleep(pTmOutMil);
                        continue;
                        }

                    pMsg = "Send Time Out";
                    break;
                }

                break;
            }
        pMsg = "Success Send Socket";
        return true;
                        }
        catch(SocketException se) 
        {
            pMsg = se.Message;
        }
        catch(Exception ex) 
        {
            pMsg = ex.Message;
        }
        
        pMsg = "Fail Send Socket::" + pMsg;
        return false;
    }

    // @@@.PLC���� ������ Tgm�� ����ó���Ѵ�.
    public bool Rcv(ref byte[] pRxBuff, 
                        int pReadCnt, 
                        SocketFlags  pFlag, 
               double  pTmOutMil, 
               ref  string  pMsg ) 
    {
        DateTime  dtRcvTm;
        int  nRemain;
        byte[] TmpRxBuff;

        try
        {
            pMsg = "";
            TmpRxBuff = new byte[pReadCnt];
            pRxBuff = new byte[pReadCnt];

            // @.WCS <-> WMS(S) ��� ���� üũ
            if (mSkt.Poll(1, SelectMode.SelectError) == true)
            {
                SktDisconnect();
                pMsg = "This Socket has an error";
                throw new Exception();
             }

            dtRcvTm = DateTime.Now.AddMilliseconds (pTmOutMil);

            while (true)
            {
                if (dtRcvTm < DateTime.Now)
                {  
                    pMsg = "Time Out";
                    break;
                }

                // @.WCS <- WMS(S) ��� ���� ���� Ȯ��
                if( mSkt.Poll(Convert.ToInt32 (pTmOutMil), SelectMode.SelectRead) == false)
                {
                    continue;
                }

                if (mSkt.Available == 0)
                {
                     SktDisconnect();
                    pMsg = "This Socket Disconnect";
                    break;
                }

                if( mSkt.Available >= pReadCnt)
                {
                    break;
                }
            }

            nRemain = mSkt.Receive(TmpRxBuff, pReadCnt, SocketFlags.None);
            System.Buffer.BlockCopy(TmpRxBuff, 0, pRxBuff, 0, nRemain);

            pMsg = "Success Read Socket";
            return true;
        }
        catch(SocketException se) 
        {
            pMsg = se.Message;
            }
        catch(Exception ex) 
        {
            pMsg = ex.Message;
        }
    
        pMsg = "Fail Read Socket::" + pMsg;
        return false;
    }

    // @@@.���� ���̵����� �б�
    public int  RcvDum(ref byte[] pByRxBuff,
                  ref string  pMsg )
    {
        int nRemain ;
        byte[] TmpRxBuff;
        int  nDum = 0;

        try
        {
            pMsg = "";
            // @.WCS <-> WMS(S) ��� ���� üũ
            if(mSkt.Poll(1, SelectMode.SelectError) == true)
            {
                this.SktDisconnect();
                pMsg = "This Socket has an error";
                throw new Exception();
            }

            // @.WCS <- WMS(S) ��� ���� ���� Ȯ��
            if( mSkt.Poll(1, SelectMode.SelectRead) == false)
            {
                throw new Exception();
            }

            if (mSkt.Available == 0)
            {
                this.SktDisconnect();
                pMsg = "This Socket Disconnect";
                throw new Exception();
            }
            else
            {
                nDum = mSkt.Available;
            }
                
            TmpRxBuff = new byte[nDum];
            pByRxBuff= new byte[nDum];

            nRemain = mSkt.Receive(TmpRxBuff, nDum, SocketFlags.None);
            System.Buffer.BlockCopy(TmpRxBuff, 0, pByRxBuff, 0, nRemain);

            pMsg = "Success Read Dummy[" + nDum.ToString() + "]";
            return  nDum;
                }
        catch (SocketException se)
        {
            pMsg = se.Message;
        }
        catch(Exception ex) {
            pMsg = ex.Message;
        
                }
        pMsg = "Fail Read Socket Dummy::" + pMsg;
        return 0;
        }
    }
}
                                                                                                                                                                                                                                     