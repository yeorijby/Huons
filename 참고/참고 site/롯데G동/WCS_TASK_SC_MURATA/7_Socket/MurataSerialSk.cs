using System;
using System.Collections.Generic;
using System.Text;

using Samoh_Lib;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Net.Sockets;
using Samoh_Socket;
using Npgsql;

namespace WCS_TASK_SC
{
    //#region enum
    //public enum MelsecQ3E_UnitType
    //{
    //    MELSECQ_CMD_WORD_UNIT = 0,
    //    MELSECQ_CMD_BIT_UNIT = 1
    //}
    //public enum MelsecQ3E_UnitType_DEVICE
    //{
    //    MELSECQ_DEVICE_CODE_SM = 0x91,
    //    MELSECQ_DEVICE_CODE_SD = 0xA9,
    //    MELSECQ_DEVICE_CODE_X = 0x9C,
    //    MELSECQ_DEVICE_CODE_Y = 0x9D,
    //    MELSECQ_DEVICE_CODE_M = 0x90,
    //    MELSECQ_DEVICE_CODE_L = 0x92,
    //    MELSECQ_DEVICE_CODE_F = 0x93,
    //    MELSECQ_DEVICE_CODE_V = 0x94,
    //    MELSECQ_DEVICE_CODE_B = 0xA0,
    //    MELSECQ_DEVICE_CODE_D = 0xA8,
    //    MELSECQ_DEVICE_CODE_W = 0xB4,
    //    MELSECQ_DEVICE_CODE_TS = 0xC1,
    //    MELSECQ_DEVICE_CODE_TC = 0xC0,
    //    MELSECQ_DEVICE_CODE_TN = 0xC2,
    //    MELSECQ_DEVICE_CODE_SS = 0xC7,
    //    MELSECQ_DEVICE_CODE_SC = 0xC6,
    //    MELSECQ_DEVICE_CODE_SN = 0xC8,
    //    MELSECQ_DEVICE_CODE_CS = 0xC4,
    //    MELSECQ_DEVICE_CODE_CC = 0xC3,
    //    MELSECQ_DEVICE_CODE_CN = 0xC5,
    //    MELSECQ_DEVICE_CODE_SB = 0xA1,
    //    MELSECQ_DEVICE_CODE_SW = 0xB5,
    //    MELSECQ_DEVICE_CODE_S = 0x98,
    //    MELSECQ_DEVICE_CODE_DX = 0xA2,
    //    MELSECQ_DEVICE_CODE_DY = 0xA3,
    //    MELSECQ_DEVICE_CODE_Z = 0xCC,
    //    MELSECQ_DEVICE_CODE_R = 0xAF,
    //    MELSECQ_DEVICE_CODE_ZR = 0xB0
    //}
    //#endregion
    #region MurataSerialSk
    public class MurataSerialSk : SocketClient
    {
        #region 변수 
#if ORACLE
        public OleDbConnection _pConObj;
        public cDbUse _pBdb;
#endif
#if POSTGRESQL
        public NpgsqlConnection _pConObj;
        public cDbPostUse _pBdb;
#endif

        private const int MELSECQ_BINARY_HEADER_LEN = 9;
        private string _strConnectionString;

        public bool m_bDBOpen;
        private string _strId;
        private string _strName;

        private int m_nSeqNum = 0;

        private int _nSubHeader = 0;
        private int _nNetworkNo = 0;
        private int _nPCNo = 0;
        private int _nRequestIONo = 0;
        private int _nRequestKookNo = 0;
        private int _nCPUWatchTimer = 0;
        //private int _nUnitType = 0;
        private string _strErrorMsg = "";
        private string _strSndHexString;
        private string _strRcvHexString;
        private string _strSndAsciiString;
        private string _strRcvAsciiString;
        private bool _Hex = true;
        private bool _Ascii = false;
        public bool IsHex { get { return _Hex; } set { _Hex = value; } }
        public bool IsAscii { get { return _Ascii; } set { _Ascii = value; } }
        public string SndHexString { get { return _strSndHexString; } set { _strSndHexString = value; } }
        public string RcvHexString { get { return _strRcvHexString; } set { _strRcvHexString = value; } }
        public string SndAsciiString { get { return _strSndAsciiString; } set { _strSndAsciiString = value; } }
        public string RcvAsciiString { get { return _strRcvAsciiString; } set { _strRcvAsciiString = value; } }
        #endregion

        #region MurataSerialSk(string ConnectionString)
        public void SetConfig_MurataSerialSk()
        {
            _nSubHeader = 0x5000;
            _nNetworkNo = 0x00;
            _nPCNo = 0xFF;
            _nRequestIONo = 0x03FF;
            _nRequestKookNo = 0x00;
            _nCPUWatchTimer = 0x0080;
        }
        public MurataSerialSk(string ConnectionString)
        {
            _strName = "MurataSerialSk";
            SetConfig_MurataSerialSk();
            _strConnectionString = ConnectionString;
        }
        //"Provider=OraOLEDB.Oracle.1; Data Source = orcl; User ID = amore_osan; Password =amore_osan"
        public MurataSerialSk(string strId, string strName, string ConnectionString)
        {
            SetConfig_MurataSerialSk();

            _strId = strId;
            _strName = strName;
            _strConnectionString = ConnectionString;
        }
        #endregion
        #region Open()
        public bool Open(ref string strRtnMsg)
        {
#if ORACLE
            string strTitle = "[Open]";

            m_bDBOpen = false;

            _pConObj = new OleDbConnection();
             try
            {

                    _pConObj.ConnectionString = _strConnectionString;
                    _pConObj.Open();

                    if (_pConObj.State != ConnectionState.Open)
                    {
                        _pConObj.Dispose();
                        strRtnMsg = strTitle + "DataBase Open이 정상적으로 이루어지지 않았습니다.";
                        return false;
                    }

                    Samoh_Lib.Cls_Shared.DB_INIT(_pConObj);
                    _pBdb = new cDbUse(ref _pConObj, false);

                m_bDBOpen = true;

                if (!Connect(ref strRtnMsg))
                {
                    strRtnMsg = strTitle + strRtnMsg;
                    return false;
                }

                m_bSocCon = true;
                return true;
            }
#endif
#if POSTGRESQL
            string strTitle = "[Open]";

            m_bDBOpen = false;


            try
            {

                _pConObj = new NpgsqlConnection();
                _pConObj.ConnectionString = _strConnectionString;
                _pConObj.Open();

                if (_pConObj.State != ConnectionState.Open)
                {
                    _pConObj.Dispose();
                    strRtnMsg = strTitle + "DataBase Open이 정상적으로 이루어지지 않았습니다.";
                    return false;
                }

                _pBdb = new cDbPostUse(_pConObj, false);


                m_bDBOpen = true;



                if (!Connect(ref strRtnMsg))
                {
                    strRtnMsg = strTitle + strRtnMsg;
                    return false;
                }

                m_bSocCon = true;
                return true;

            }
#endif
            catch (Exception e)
            {
                strRtnMsg = strTitle + e.ToString();
                _pConObj.Dispose();
                return false;
            }
            
        }
        #endregion
        #region Close()
        public void Close(ref string strRtnMsg)
        {
            try
            {
                if (m_bSocCon)
                {
                    string msg = "";
                    ThreadStop(ref msg);
                }
                if (m_bDBOpen)
                {
                    _pConObj.Close();
                    _pConObj.Dispose();
                    m_bDBOpen = false;
                }

                strRtnMsg = "[Close] DB, Socket Close. Success"; 
            }
            catch (SocketException sex)
            {
                strRtnMsg = "[Close] DB, Socket Close중 Socket Exception Message [" + sex.ToString() +"]"; 
                return;
            }
            catch (Exception ex)
            {
                strRtnMsg = "[Close] DB, Socket Close중 Socket Exception Message [" + ex.ToString() + "]"; 
                return;
            }
        }
        #endregion Close()
        #region READ,WRITE

        public bool CmdReq(string strMsgType, string strData = "", string strResData = "")
        {
            byte[] byTxBuff = new byte[1000];

            Array.Clear(byTxBuff, 0x00, byTxBuff.Length);

            int nSeqNo = GetSeqNum();

            byTxBuff[0] = cDefApp.STX;

            byte c1st = (byte)(nSeqNo / 100);

            string cvtvalue = "";
            cvtvalue = string.Format("{0:D3}", nSeqNo);
            byte[] byteSeqNo = StringToByte(cvtvalue);
            byte[] byteMsgType = StringToByte(strMsgType);

            int iii = 0;
            byTxBuff[iii++] = cDefApp.STX;
            if (strMsgType == "000" || strMsgType == "999")
            {
                foreach (byte b in byteMsgType)
                {
                    byTxBuff[iii++] = b;
                }
            }
            else
            {
                foreach (byte bb in byteSeqNo)
                {
                    byTxBuff[iii++] = bb;
                }

                // 상태요구 응답 or 에러요구 응답 일경우 아래와 같이 처리 
                if (strResData != "")
                {
                    foreach (byte bbbb in strResData)
                    {
                        byTxBuff[iii++] = bbbb;
                    }
                    //byTxBuff[iii++] = cDefApp.ETX;
                    //byTxBuff[iii] = MakeBcc(byTxBuff, 1, iii);
                    //return true;        // 여기서 리턴해야함!
                    goto EXIT_LBL;
                }

                foreach (byte b in byteMsgType)
                {
                    byTxBuff[iii++] = b;
                }

                if (strMsgType.Substring(0,1) == "B")
                {
                    if (strData == "")
                    {
                        // 작업지시일때 DATA 메세지가 없으면 안됨
                        SetErrorMsg("Request.. 송신 에러 - 작업지시시에 작업 데이터가 없음! [CMD:" + strMsgType + "][DATA:" + strData + "]");
                        return false;
                    }

                    foreach (byte bbb in strData)
                    {
                        byTxBuff[iii++] = bbb;
                    }
                }
            }

            EXIT_LBL:
            {
                byTxBuff[iii++] = cDefApp.ETX;
                byTxBuff[iii] = MakeBcc(byTxBuff, 1, iii);

                SndHexString = "";
                if (IsHex)
                {
                    SndHexString = BytesToHexs(byTxBuff, ++iii);
                }
                if (IsAscii)
                {
                    SndAsciiString = Encoding.Default.GetString(byTxBuff, 0, ++iii);
                }

                Clearbuffer();
                string msg = "";
                if (!SendRst(byTxBuff, iii, ref msg))
                {
                    SetErrorMsg("Request.. 송신 에러 [" + msg + "]");
                    return false;
                }

                //return RecvReadAck(nUnitType, 11 + GetDataLength(nUnitType, nReadLen), ref RXBUFF);

                return true;
            }
        }

        // strCmd : StatusRead => "A1", ErrorRead => "A2",
        // strCmd : Move => "B1", Store => "B2", Retrieve => "B3", CellToCell => "B4",
        // strCmd : DestChange(Cell) => "B5", DestChange(ST) => "B6"
        // strCmd : StartOn => "D1", Clear => "D2", ReturnHP => "D3", ErrorReset => "D4"
        public bool ReadReqeust(string strCmd, ref byte[] RXBUFF)
        {
            if (CmdReq(strCmd) == false)
            {
                //SetErrorMsg("Request.. 송신 에러 [" + msg + "]");         // - 이미 함수 안에서 다 로그 찍음!
                return false;
            }

            byte[] byRxBuff = new byte[1000];

            int nReadLen = 0;
            Array.Clear(byRxBuff, 0x00, byRxBuff.Length);

            if (RecvStxToEtx(0, ref nReadLen, ref byRxBuff) == false)       // ????
            {
                //SetErrorMsg("Request.. 송신 에러 [" + msg + "]");         // - 이미 함수 안에서 다 로그 찍음!
                return false;
            }

            if (nReadLen == 0)
                return false;
            if (byRxBuff[nReadLen - 2] != cDefApp.ETX)
                return false;
            if (byRxBuff[0] != cDefApp.STX)
                return false;

            Buffer.BlockCopy(byRxBuff, 0, RXBUFF, 0, nReadLen);
            return true;
        }

        public bool READ(int nUnitType
            , byte DeviceCode
            , int nStartAddr
            , int nReadLen
            , ref byte[] RXBUFF)
        {
            byte[] byTxBuff = new byte[1000];

            Array.Clear(byTxBuff, 0x00, byTxBuff.Length);

            byTxBuff[0] = (byte)(_nSubHeader / 0x100);
            byTxBuff[1] = (byte)(_nSubHeader % 0x100);

            byTxBuff[2] = (byte)(_nNetworkNo);	                        // Network No. (Self station)
            byTxBuff[3] = (byte)(_nPCNo);     	                        // PC No.

            byTxBuff[4] = (byte)(_nRequestIONo % 0x100);
            byTxBuff[5] = (byte)(_nRequestIONo / 0x100);	            // Specific value
            byTxBuff[6] = (byte)(_nRequestKookNo % 0x100);	            // Specific value 2

            byTxBuff[7] = 0x0C;
            byTxBuff[8] = 0x00;	                                        // Request data length

            byTxBuff[9] = (byte)(_nCPUWatchTimer % 0x100);              // 0x04;
            byTxBuff[10] = (byte)(_nCPUWatchTimer / 0x100);	            // CPU monitoring timer (unit is 250ms)

            byTxBuff[11] = 0x01;
            byTxBuff[12] = 0x04;                                        //Command
            byTxBuff[13] = (byte)(nUnitType);
            byTxBuff[14] = 0x00; 	                                    // Subcommand

            byTxBuff[15] = (byte)((nStartAddr & 0xFF));
            byTxBuff[16] = (byte)((nStartAddr >> 8) & 0xFF);
            byTxBuff[17] = (byte)((nStartAddr >> 16) & 0xFF);		    // Head device (Start Address)

            byTxBuff[18] = DeviceCode;					                // Device code (Memory Type)

            byTxBuff[19] = (byte)(nReadLen & 0x00FF);				    //WORD단위:읽을 워드수, BIT단위:읽을 비트수
            byTxBuff[20] = (byte)((nReadLen >> 8) & 0x00FF);		    // Number of device points


            SndHexString = "";
            if (IsHex)
            {
                SndHexString = BytesToHexs(byTxBuff, 21);
            }
            if (IsAscii)
            {
                SndAsciiString = Encoding.Default.GetString(byTxBuff, 0, 21);
            }

            Clearbuffer();
            string msg = "";
            if (!SendRst(byTxBuff, 21, ref msg))
            {
                SetErrorMsg("Read.. 송신 에러 [" + msg + "]");
                return false;
            }

            return RecvReadAck(nUnitType, 11 + GetDataLength(nUnitType, nReadLen), ref RXBUFF);
        }
        public bool WRITE(int nUnitType, byte DeviceCode, int nStartAddr, int nWriteLen, byte[] TXBUFF)
        {
            byte[] byTxBuff = new byte[1000];

            Array.Clear(byTxBuff, 0x00, byTxBuff.Length);


	        long wReqLen = 12 + GetDataLength(nUnitType, nWriteLen);

            byTxBuff[0]  = (byte)(_nSubHeader / 0x100);  
	        byTxBuff[1]  = (byte)(_nSubHeader % 0x100);

	        byTxBuff[2]  = (byte)(_nNetworkNo);	                // Network No. (Self station)
	        byTxBuff[3]  = (byte)(_nPCNo);     	                // PC No.

            byTxBuff[4]  = (byte)(_nRequestIONo % 0x100);	
	        byTxBuff[5]  = (byte)(_nRequestIONo / 0x100);	    // Specific value
	        byTxBuff[6]  = (byte)(_nRequestKookNo % 0x100);	    // Specific value 2

	        byTxBuff[7]  = (byte)(wReqLen & 0x00FF);
	        byTxBuff[8]  = (byte)((wReqLen >> 8) & 0x00FF);  	// Request data length

	        byTxBuff[9]  = (byte)(_nCPUWatchTimer % 0x100);         // 0x04;
	        byTxBuff[10] = (byte)(_nCPUWatchTimer / 0x100);	        // CPU monitoring timer (unit is 250ms)

	        byTxBuff[11] = 0x01;
	        byTxBuff[12] = 0x14;                                    //Command
	        byTxBuff[13] = (byte)(nUnitType);
	        byTxBuff[14] = 0x00; 	                                // Subcommand

	        byTxBuff[15] = (byte)((nStartAddr & 0xFF));
	        byTxBuff[16] = (byte)((nStartAddr >> 8) & 0xFF);
	        byTxBuff[17] = (byte)((nStartAddr >> 16) & 0xFF);	    // Head device (Start Address)

	        byTxBuff[18] = DeviceCode;					            // Device code (Memory Type)

	        byTxBuff[19] = (byte)(nWriteLen & 0x00FF);              //WORD단위:쓸 워드수, BIT단위:쓸 비트수
            byTxBuff[20] = (byte)((nWriteLen >> 8) & 0x00FF);       //Number of device points



            SndHexString = "";
            if (IsHex)
            {
                SndHexString = BytesToHexs(byTxBuff, 21);
            }
            if (IsAscii)
            {
                SndAsciiString = Encoding.Default.GetString(byTxBuff, 0, 21);
            }

            int nLen = GetDataLength(nUnitType, nWriteLen);
            Buffer.BlockCopy(TXBUFF, 0, byTxBuff, 21, nLen);


            string msg = "";
	        Clearbuffer();
            if (!Write(ref byTxBuff, 21 + nLen, ref msg))
	        {
		        SetErrorMsg("Write.. 송신 에러 [" + msg +"]");
		        return false;
	        }

            return RecvWriteAck(ref msg);
        }
        #endregion
        #region 내부함수
        public byte MakeBcc(byte[] Text, int nTextStart, int nTextLen)
        {
            int i;
            byte Bcc = 0;

            for (i = 0; i < nTextLen; i++)
                //Bcc ^= *(Text + i);
                Bcc ^= Text[i+ nTextStart];

            return (Bcc);
        }
        public int GetSeqNum()
        {
            if (m_nSeqNum > 255 || m_nSeqNum < 1)
                m_nSeqNum = 1;

            return m_nSeqNum++;
        }
        private byte[] StringToByte(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            return bytes;
        }
        private string ByteToString(byte[] bytes)
        {
            string str = Encoding.Default.GetString(bytes);
            return str;
        }
        public bool Write(ref byte[] pTxBuff, int nTxLen, ref string msg)
        {
            ResetError();
            if (!SendRst(pTxBuff, nTxLen, ref msg))
            {
                ThreadStop(ref msg);

                return false;
            }


	        return true;
        }
        public void ResetError()
        {
	        _strErrorMsg = "";
        }
        public void SetErrorMsg(string strMsg)
        {
	        _strErrorMsg = strMsg;
        }
        public bool RecvStxToEtx(int nUnitType, ref int nReadLen, ref byte[] RXBUFF)
        {
            byte[] byRxBuff = new byte[3000];
            Array.Clear(byRxBuff, 0x00, byRxBuff.Length);

            nReadLen = 0;
            RcvHexString = "";
            string msg = "";
            if (!RecvRstLoop(ref byRxBuff, ref msg, 10))
            {
                if (IsHex && RcvLen > 0)
                {
                    RcvHexString = BytesToHexs(byRxBuff, RcvLen);
                }
                if (IsAscii && RcvLen > 0)
                {
                    RcvAsciiString = Encoding.Default.GetString(byRxBuff, 0, RcvLen);
                }

                SetErrorMsg("Recv.. 에러 [REC_STR : " + RcvHexString + "]");      // HEX 이므로...
                nReadLen = byRxBuff.Length;
                return false;
            }

            if (IsHex && RcvLen > 0)
            {
                RcvHexString = BytesToHexs(byRxBuff, RcvLen);
            }
            if (IsAscii && RcvLen > 0)
            {
                RcvAsciiString = Encoding.Default.GetString(byRxBuff, 0, RcvLen);
            }

            nReadLen = byRxBuff.Length;
            Buffer.BlockCopy(byRxBuff, 11, RXBUFF, 0, nReadLen - 2);
            return true;
        }
        public bool RecvReadAck(int nUnitType, int nReadLen, ref byte[] RXBUFF)
        {
            byte[] byRxBuff = new byte[3000];
            Array.Clear(byRxBuff, 0x00, byRxBuff.Length);


            RcvHexString = "";
            string msg = "";
            if (!RecvRst(ref byRxBuff, nReadLen, ref msg)) 
            {
                if (IsHex && RcvLen>0)
                {
                    RcvHexString = BytesToHexs(byRxBuff, RcvLen);
                }
                if (IsAscii && RcvLen > 0)
                {
                    RcvAsciiString = Encoding.Default.GetString(byRxBuff, 0, RcvLen);
                }
                return false;
            }

            if (IsHex && RcvLen > 0)
            {
                RcvHexString = BytesToHexs(byRxBuff, RcvLen);
            }
            if (IsAscii && RcvLen > 0)
            {
                RcvAsciiString = Encoding.Default.GetString(byRxBuff, 0, RcvLen);
            }

	        if(byRxBuff[0] != 0xD0 || byRxBuff[1] != 0x00)                //Subheader
	        {
		        SetErrorMsg("RecvReadAck.. Subheader 이상..");
		        return false;
	        }

	        if(byRxBuff[2] != 0x00 || byRxBuff[3] != 0xFF)                // Network NO, PC NO
	        {
		        SetErrorMsg("RecvReadAck.. Network or PC NO 이상..");
		        return false;
	        }
	        if(byRxBuff[4] != 0xFF || byRxBuff[5] != 0x03 || byRxBuff[6] != 0x00)   // Specific value
	        {
		        SetErrorMsg("RecvReadAck.. Specific Value 이상..");
		        return false;
	        }


            long wErrNo = SwapToWord(byRxBuff[9], byRxBuff[10]);
	        if(wErrNo>0)
	        {
		        SetErrorMsg("RecvReadAck.. 응답 수신.. 오류코드[" + wErrNo.ToString() + "]");
		        return false;
	        }


            Buffer.BlockCopy(byRxBuff, 11, RXBUFF, 0, nReadLen - 2);


	        return true;
        }
        public bool RecvWriteAck(ref string msg)
        {
            byte[] byRxBuff = new byte[2000];
            Array.Clear(byRxBuff, 0x00, byRxBuff.Length);

            RcvHexString = "";
            if (RecvRst(ref byRxBuff, MELSECQ_BINARY_HEADER_LEN, ref msg) == false)
            {
                if (IsHex && RcvLen > 0)
                {
                    RcvHexString = BytesToHexs(byRxBuff, RcvLen);
                }
                if (IsAscii && RcvLen > 0)
                {
                    RcvAsciiString = Encoding.Default.GetString(byRxBuff, 0, RcvLen);
                }
                return false;
            }

            if (IsHex && RcvLen > 0)
            {
                RcvHexString = BytesToHexs(byRxBuff, RcvLen);
            }
            if (IsAscii && RcvLen > 0)
            {
                RcvAsciiString = Encoding.Default.GetString(byRxBuff, 0, RcvLen);
            }

            if (byRxBuff[0] != 0xD0 || byRxBuff[1] != 0x00)                          //Subheader
            {
                SetErrorMsg("RecvReadAck.. Subheader 이상..");
                return false;
            }
            if (byRxBuff[2] != 0x00 || byRxBuff[3] != 0xFF)                // Network NO, PC NO
            {
                SetErrorMsg("RecvWriteAck.. Network or PC NO 이상..");
                return false;
            }
            if (byRxBuff[4] != 0xFF || byRxBuff[5] != 0x03 || byRxBuff[6] != 0x00)   // Specific value
            {
                SetErrorMsg("RecvWriteAck.. Specific Value 이상..");
                return false;
            }

            return true;
        }
        private long SwapToWord(byte pX, byte pY)
        {
            return (long)(pY << 8) | pX;
        }
        //private long SwapToDWord(ref byte[] pSrc)
        //{
        //    return (long)(pSrc[3] << 24) | (pSrc[2] << 16) | (pSrc[1] << 8) | pSrc[0];
        //}
        public string ConvertErrorMsg(int nErrorCode)
        {
	        switch(nErrorCode)
	        {
		        case 0x10: return "PC Number error...";
		        case 0x11: return "Mode error...";
		        case 0x12: return "Special-function module specification error...";
		        case 0x13: return "Program step No. specification error...";
		        case 0x18: return "Remote error...";
		        case 0x21: return "Special-function module bus error...";
	        }

	        return "";
        }
        public int GetDataLength(int nUnitType, int wLen)
        {
            if (nUnitType == (int)MelsecQ3E_UnitType.MELSECQ_CMD_WORD_UNIT)
            {
                return (wLen * 2);
            }
            else
            {
                return ((wLen / 2) + (wLen % 2));
            }
        }
        #endregion 
    }
    #endregion
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         