using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Samoh_Lib;
using System.Data;
using System.Data.OleDb;
using log4net;
using log4net.Config;
using System.Windows.Forms;
using NpgsqlTypes;

namespace WCS_TASK_SC
{
	//2014 ������ �޸𸮿� Cv ���¸� �����Ѵ�.
	public class SCData
	{
		private string HexaScValue;     // m_strStatusCheck
		public string HEXA_SC_VALUE
		{
			get { return HexaScValue; }
			set { HexaScValue = value; }
		}

        private int ScErrCd;            // WORD m_wErrCode
        public int ERR_CODE_RD
        {
            get { return ScErrCd; }
            set { ScErrCd = value; }
        }

        private string OpMode;          // BOOL m_bOperatingMode
        public string OP_MODE
        {
            get { return OpMode; }
            set { OpMode = value; }
        }



        // MURATA
        private bool StartOn;
        public bool START_ON
        {
            get { return StartOn; }
            set { StartOn = value; }
        }

        private bool Invoke;
        public bool INVOKE
        {
            get { return Invoke; }
            set { Invoke = value; }
        }


        private bool Error;
        public bool EROOR
        {
            get { return Error; }
            set { Error = value; }
        }


        private bool ComeBackHome;
        public bool COMEBACKHOME
        {
            get { return ComeBackHome; }
            set { ComeBackHome = value; }
        }


        private bool HomeStop;
        public bool HOMESTOP
        {
            get { return HomeStop; }
            set { HomeStop = value; }
        }


        private bool Transaction;
        public bool TRANSACTION
        {
            get { return Transaction; }
            set { Transaction = value; }
        }


        private bool ProdCheck;
        public bool PROD_CHECK
        {
            get { return ProdCheck; }
            set { ProdCheck = value; }
        }


        private bool OrderCheck;
        public bool ORDER_CHECK
        {
            get { return OrderCheck; }
            set { OrderCheck = value; }
        }


        /*
        private string OnlineModeRd;
        public string ONLINE_MODE_RD 
        {
            get { return OnlineModeRd; }
            set { OnlineModeRd = value; }
        }

        private string AutoModeRd;
        public string AUTO_MODE_RD
        {
            get { return AutoModeRd; }
            set { AutoModeRd = value; }
        }

        private string UcstatusRd;
        public string UCSTATUS_RD
        {
            get { return UcstatusRd; }
            set { UcstatusRd = value; }
        }

        private string ActiveModeRd;
        public string ACTIVE_MODE_RD
        {
            get { return ActiveModeRd; }
            set { ActiveModeRd = value; }
        }
        //*/
        public SCData()
		{
			HEXA_SC_VALUE = "";
            OP_MODE = "";
		}
	}

    public class ScThread : maindefine
    {
        public PsMsgView callPsMsgView = null;
        public PfSetStatImgViewDB callPicDb = null;
        public PfSetStatImgViewSOCKET callPicSocket = null;

        public int m_nThNo = 0;
        public string m_strWh_typ = "";
        public string m_strEqmtTyp = "";
        public string m_strPlc_No = "";
        public string m_strScNo = "";
        public string m_strMcNo = "";
        public string m_strScGrpNo = "";
        public string m_strIp = "";
        public string m_strCurPort = "";
        public string m_strFromPort = "";
        public string m_strToPort = "";
        public int m_nPortCnt = 0;
        public string m_strConnectString = "";
        public string m_strLogFileNm = "";
        public int m_nCnt = 0;
        public int m_nCurPort = 0;
        public int m_nFromPort = 0;
        public int m_nToPort = 0;
        int nSelCnt = 0;
        public string m_strLogMsg = "";
        public bool m_blHostErrSendYN = false;
        public bool m_blHostSendYN = false;
        public bool m_blConnectYn = false;
       

        public string m_firstErr = "";
        public int m_firstErrChk = 0;
        public int m_nRetCd = 0;
        private string _strErrorMsg = "";
        public string m_strRtnMsg;
        //private MelsecQ3EProtocol m_msQPlc;
        private MurataSerialSk m_msQPlc;
        public Thread m_thThread;
        public SYS_MAIN m_frmMain;
        private bool m_bOpen;
        public bool IsOpen { get { return m_bOpen; } set { m_bOpen = value; } } //���α׷� ȭ��ǥ�ÿ�.

        public PictureBox m_picStatOp = null;
        public PictureBox m_picStatDbCn = null;

		//Dictionary ��ü�� ������.
		//2014 ������.
		Dictionary<int, SCData> ScDic = new Dictionary<int, SCData>();

		string strSql = "";
		string CRLF = "\r\n";
		int ReqCnt = 0;

        public ScThread(int nThNo,
						string strWh_Typ,
                        string strEqmtTyp,
                        string strPlc_No,
                        string strSc_No,
                        string strMc_No,
                        string strSc_Grp_No,
                        string strIp,
                        string strCurPort,
                        string strFromPort,
                        string strToPort,
                           int nPortCnt,
                        string strConnectString,
                        string strLogFileNm)
        {
            m_nThNo = nThNo;
            m_strWh_typ = strWh_Typ;
            m_strEqmtTyp = strEqmtTyp;
            m_strPlc_No = strPlc_No;
            m_strScNo = strSc_No;
            m_strMcNo = strMc_No;
            m_strScGrpNo = strSc_Grp_No;
            m_strIp = strIp;
            m_strCurPort = strCurPort;
            m_strFromPort = strFromPort;
            m_strToPort = strToPort;

            m_nCurPort = Convert.ToInt32(0 + m_strCurPort);
            m_nFromPort = Convert.ToInt32(0 + m_strFromPort);
            m_nToPort = Convert.ToInt32(0 + m_strToPort);

            m_strConnectString = strConnectString;
            m_strLogFileNm = strLogFileNm;
            m_nPortCnt = nPortCnt;

            IsOpen = false;
            m_msQPlc = new MurataSerialSk(m_strConnectString);
            m_msQPlc.IsHex = true;
        }

        /*
         * ȭ�� ǥ�ÿ�
         */
        #region ȭ�� ǥ�ÿ�.
        private void MakeMsg(string msg, int nThGbn)
        {
            try
            {
                m_frmMain.PsMsgView(msg, m_strPlc_No.ToString(), nThGbn);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void MakeMsg_Error(string msg, int nThGbn)
        {
            try
            {
                m_frmMain.PsMsgView_Error(msg, m_strPlc_No.ToString(), nThGbn);
                cDefApp.m_LogQ[m_nThNo].Enqueue(new LogParam(DateTime.Now, msg));
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void MakeMsg_Imp(string msg, int nThGbn)
        {
            try
            {
                m_frmMain.PsMsgView_IMP(msg, m_strPlc_No.ToString(), nThGbn);
                cDefApp.m_LogQ[m_nThNo].Enqueue(new LogParam(DateTime.Now, msg));
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void SetErrorMsg(string strMsg)
        {
            _strErrorMsg = strMsg;
            Log.Error(_strErrorMsg);
        }

        private void PicDBChange(string strDBStat)
        {
            callPicDb(m_picStatDbCn, strDBStat);
        }

        private void PicSocketChange(string strStatSocket, string strStatOp)
        {
            callPicSocket(m_picStatOp, strStatSocket, strStatOp);
        }
        #endregion
        /* 
         * �Ǳ�����
         */
        #region Thread_Doing
        public void Thread_Doing(object value)
        {
            string strTitle = "[Thread_Doing]";
            try
            {
                MakeMsg_Imp("DB/Socket Connectting", m_nThNo);
#if SOCKET
                    if (m_msQPlc.m_bSocCon == false && m_msQPlc.m_bDBOpen == false)
                    {
                        // open�� ��Ʈ���� ��ŭ �翬��
                        for (int i = 0; i < m_nToPort - m_nFromPort; i++)
                        {
                            if (m_nCurPort > m_nToPort)
                            {
                                m_nCurPort = m_nFromPort;
                            }
                            for (int j = 0; j < m_nPortCnt; j++)
                            {
                                MakeMsg_Imp(string.Format("IP [{0}] PORT [{1}] ���ӽõ�", m_strIp, m_nCurPort.ToString()), m_nThNo);
                                m_msQPlc.SetConfig(m_strIp, m_nCurPort, 2);

                                if (!m_msQPlc.Open(ref m_strRtnMsg))
                                {
                                    SetErrorMsg("Comm" + m_nThNo + " :" + m_strRtnMsg);
                                    MakeMsg_Error(m_strRtnMsg, m_nThNo);

                                    //DB�� ���� �Ǿ��µ� ����� ������ �ȵǾ� �ִ� ��� LOG�����
                                    if (m_msQPlc.m_bSocCon == false && m_msQPlc.m_bDBOpen == true)
                                    {
                                        InsertWcsLogPgr("", "[Thread_Doing] ���� ���� ����");
                                    }

                                    m_msQPlc.Close(ref m_strRtnMsg);

                                    if (j == m_nPortCnt - 1)
                                    {
                                        m_nCurPort = m_nCurPort + 1;
                                    }
                                    m_blConnectYn = false;

                                    Thread.Sleep(2000);
                                    continue;
                                }
                                else
                                {
                                    // ini�� ���� ������ ��Ʈ�� ����
                                    string strCOMM = "COMM" + m_nThNo;
                                    cDefApi.WritePrivateProfileString(strCOMM, "CUR_PORT", Convert.ToString("" + m_nCurPort), cDefApp.GM_ENV_INI);

                                    //���� ���� �α� ����� 
                                    InsertWcsLogPgr("", "[Thread_Doing] SC ��ȣ : " + m_strScNo + ", ������Ʈ : " + m_nCurPort + " ���� ����");

                                    //������ �����ϰų� �õ�Ƚ���� OVER�ϸ� ����������.
                                    m_blConnectYn = true;
                                    break;
                                }
                            }
                            //������ �����ϸ� ����������.
                            if (m_blConnectYn == true)
                            {
                                break;
                            }
                        }
                        //Thread.Sleep(5000); 
                    }
#endif
#if SERIAL
                if (m_msQPlc.m_bDBOpen == false)
                {
                    if (!m_msQPlc.Open(ref m_strRtnMsg, false))
                    {
                        SetErrorMsg("Comm" + m_nThNo + " :" + m_strRtnMsg);
                        MakeMsg_Error(m_strRtnMsg, m_nThNo);

                        ////DB�� ���� �Ǿ��µ� ����� ������ �ȵǾ� �ִ� ��� LOG�����
                        //if (m_msQPlc.m_bSerialCon == false && m_msQPlc.m_bDBOpen == true)
                        //{
                        InsertWcsLogPgr("", "[Thread_Doing] DB ���� ����");
                        //}

                        m_msQPlc.Close(ref m_strRtnMsg);

                        m_blConnectYn = false;

                        Thread.Sleep(2000);
                    }
                    else
                    {
                        // DB ���� ���� �ÿ�... => �ø��� ����!! 
                        if (!m_frmMain.SerialPort_Open(ref m_strRtnMsg))
                        {
                            m_msQPlc.m_bSerialCon = false;

                            SetErrorMsg("Comm" + m_nThNo + " :" + m_strRtnMsg);
                            MakeMsg_Error(m_strRtnMsg, m_nThNo);
                            throw new Exception(m_strRtnMsg);
                        }

                        m_msQPlc.m_bSerialCon = true;


                        // ini�� ���� ������ ��Ʈ�� ����
                        string strCOMM = "COMM" + m_nThNo;
                        cDefApi.WritePrivateProfileString(strCOMM, "CUR_PORT", Convert.ToString("" + m_nCurPort), cDefApp.GM_ENV_INI);

                        //���� ���� �α� ����� 
                        InsertWcsLogPgr("", "[Thread_Doing] SC ��ȣ : " + m_strScNo + ", ������Ʈ : " + m_nCurPort + " ���� ����");

                        //������ �����ϰų� �õ�Ƚ���� OVER�ϸ� ����������.
                        m_blConnectYn = true;
                        //break;
                    }
                }
#endif
                #region DB�� ���� �� ���� �Ǿ��� �� 
                if ((m_msQPlc.m_bSocCon == true || m_msQPlc.m_bSerialCon == true) && 
                     m_msQPlc.m_bDBOpen == true)
                {
                    IsOpen = true;
                    MakeMsg_Imp("DB login Ok!", m_nThNo);

                    while (true)
                    {

                        if (cDefApp.GM_STAT_MAIN == false)
                        {
                            throw new Exception("���� �����");
                        }
                        this.m_msQPlc.IsAscii = m_frmMain.IsAscii;
                        this.m_msQPlc.IsHex = m_frmMain.IsHex;

                        if (!MurataScReadStatus()) goto EXIT_LBL;

                        // ��ȭ���� ��� - ����Ÿ������ ��ȭ���ڿ� ���� ����� ����! 
                        // ��ȭ���ڷδ� ������ �ִµ� ������ IO_SCH���� �����ص� �ɵ�!
                        //if (!SC_CMD_RQ_YN()) goto EXIT_LBL;

                        // �۾����ø��
                        // ���(B1, B2, ...)���� JOB_TYPE���� �����!
                        if (!MURATA_SC_OD_RQ_YN()) goto EXIT_LBL;

                        Thread.Sleep(200); //2000
                    }
                }
            #endregion
            EXIT_LBL:
                {
                    SetErrorMsg("CoMM" + m_nThNo + " DB & Socket logoff!");
                    MakeMsg_Imp("DB & Socket logoff!", m_nThNo);
                }

            }
            catch (Exception ex)
            {
                MakeMsg_Error(ex.Message, m_nThNo);
            }

            IsOpen = false;

            m_msQPlc.Close(ref m_strRtnMsg);
            MakeMsg_Imp(m_strRtnMsg, m_nThNo);

            m_frmMain.SerialPort_Close(ref m_strRtnMsg);
            MakeMsg_Imp(m_strRtnMsg, m_nThNo);

            m_thThread = null;
            m_blConnectYn = false;
        }
        #endregion Thread_Doing

        #region[MurataScReadStatus] :: SC READ �� ���� �� DB�� �Է�
        private bool MurataScReadStatus()
        {
            string strTitle = "[MurataScReadStatus]";
            int nCount = 0;

            try
            {
                byte[] byRxBuff = new byte[2000];
                int nReadLenth = 36; //D95~130
                int nReadAddress = 95; //D96����

                MakeMsg(strTitle + "SC ���", m_nThNo);

                Array.Clear(byRxBuff, 0x00, byRxBuff.Length);

                bool bResult = false;

                //bResult = m_msQPlc.ReadReqeust("A1", ref byRxBuff);
#if SOCKET
                bResult = m_msQPlc.ReadReqeust("A1", ref byRxBuff);
#else
                // ���� �޼��� �����... 
                int nLen = 0;
                byte[] byTxBuff = new byte[1000];
                string strTxMsg = "";
                if (m_msQPlc.CmdReq("A1", ref strTxMsg, ref byTxBuff, ref nLen) == false)
                {
                    //SetErrorMsg("Request.. �۽� ���� [" + msg + "]");         // - �̹� �Լ� �ȿ��� �� �α� ����!
                    return false;
                }
                //string strTxMsg = m_msQPlc.ByteToString(byTxBuff);

                //strTxMsg = strTxMsg.Substring(0, nLen);
                //string strTxMsg = m_msQPlc.ByteToString();
                // �ø���� ������
                bResult = m_frmMain.SendSerialData(strTxMsg, ref m_strLogMsg);

                Thread.Sleep(300);
                string strTemp = "";
                if (bResult == true && m_frmMain.m_bSerialDataReceived == true)
                {
                    nCount = 0;
                       
                    for (int iiii = 0; iiii < m_frmMain.m_nSerialMsgCnt; iiii++)
                    {
                        strTemp = m_frmMain.m_strSerialMsg[iiii];

                        MakeMsg("Serial Received ... [" + strTemp + "]", m_nThNo);
                    }
                    byRxBuff = Encoding.UTF8.GetBytes(strTemp);
                    --m_frmMain.m_nSerialMsgCnt;
                    m_frmMain.m_bSerialDataReceived = false;

                    // �޾ƿ°��� ������ ��������
                    //break;
                }
#endif

                // �޾ƿ°� Ȯ���ϱ�
                if (bResult == false)
                {
                    if (++m_nCnt == 3)
                    {
                        UpdateMURATA_SC_DATA("0"       // strStatusResErrCode
                                           , "100"     // strErrorCode
                                           , "0"       // strStatus
                                           , "0"       // strCraneOnline
                                           , "0"       // strCraneRequest
                                           , "1"       // strIsError
                                           , "0"       // strComeBackHome
                                           , "0"       // strCraneHP
                                           );

                        m_msQPlc.Close(ref m_strLogMsg);
                        m_nCnt = 0;
                    }
                    //���� ��Ż��� ������Ʈ(N)
                    Communication("N", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);
                    throw new Exception();
                }
                m_nCnt = 0;


                //���� ��Ż��� ������Ʈ(Y)
                Communication("Y", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);

                int nReadLen = 70;

                MakeMsg("���°� DB����", m_nThNo);

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////
                string strRxBuff = m_msQPlc.BytesToHexs(byRxBuff, byRxBuff.Length);

                if (strRxBuff[0] != cDefApp.STX)
                {
                    MakeMsg_Error(strTitle + "STX ����", m_nThNo);
                    return false;
                }

                string strSeqNo = strRxBuff.Substring(1, 3);
                //if (strSeqNo != cDefApp.STX)
                //{
                //    MakeMsg_Error(strTitle + "SeqNo ����", m_nThNo);
                //    return false;
                //}

                string strStatusResErrCode = strRxBuff.Substring(4, 2);




                if (!ScDic.ContainsKey(m_nThNo))
                {
                    ScDic.Add(m_nThNo, new SCData()); //Key�� �߰��Ѵ�.
                }

                #region �ּ�.
                /*
                ////���� ���°� DIC�� �ֱ�.
                //if (ScDic[m_nThNo].ONLINE_MODE_RD == "" &&
                //    ScDic[m_nThNo].AUTO_MODE_RD == "" &&
                //    ScDic[m_nThNo].UCSTATUS_RD == "" &&
                //    ScDic[m_nThNo].ACTIVE_MODE_RD == "" &&
                //    ScDic[m_nThNo].ERR_CODE_RD == 0)
                //{
                if (//ScDic[m_nThNo].OP_MODE == "" &&
                    //ScDic[m_nThNo].HEXA_SC_VALUE == "" &&
                    ScDic[m_nThNo].START_ON == false &&
                    //ScDic[m_nThNo].INVOKE == "" &&
                    //ScDic[m_nThNo].EROOR == "" &&
                    //ScDic[m_nThNo].COMEBACKHOME == "" &&
                    //ScDic[m_nThNo].TRANSACTION == "" &&
                    //ScDic[m_nThNo].PROD_CHECK == "" &&
                    //ScDic[m_nThNo].ORDER_CHECK == "" &&
                    ScDic[m_nThNo].ERR_CODE_RD == 0)
                {
                        int nSelCnt = 0;

                    strSql = "";
                    strSql += CRLF = "SELECT *                   ";
                    strSql += CRLF = "  FROM SC_DATA_MURATA      ";
                    strSql += CRLF = " WHERE WH_TYP = :WH_TYP    ";
                    strSql += CRLF = "   AND WH_TYP = :WH_TYP    ";
                    strSql += CRLF + "   AND PLC_NO = :PLC_NO    ";
                    strSql += CRLF + "   AND SC_NO  = :SC_NO     ";

                    m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                    m_msQPlc._pBdb.mComMain.Parameters.Clear();
                    m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                    m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                    m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;

                    nSelCnt = m_msQPlc._pBdb.ExcuteQry(strSql);

                    if (nSelCnt < 0)
                    {
                        MakeMsg_Error(strTitle + "���� SC ���� �д� �� ����(SC_DATA)", m_nThNo);
                        return false;
                    }

                    //ScDic[m_nThNo].ONLINE_MODE_RD = m_msQPlc._pBdb.mDtMain.Rows[0]["ONLINE_MODE_RD"].ToString();
                    //ScDic[m_nThNo].AUTO_MODE_RD = m_msQPlc._pBdb.mDtMain.Rows[0]["AUTO_MODE_RD"].ToString();
                    //ScDic[m_nThNo].UCSTATUS_RD = m_msQPlc._pBdb.mDtMain.Rows[0]["UCSTATUS_RD"].ToString();
                    //ScDic[m_nThNo].ACTIVE_MODE_RD = m_msQPlc._pBdb.mDtMain.Rows[0]["ACTIVE_MODE_RD"].ToString();

                    ScDic[m_nThNo].START_ON = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[0]["CRANE_STATUS_RD"].ToString());    
                    ScDic[m_nThNo].ERR_CODE_RD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[0]["ERR_CODE_RD"].ToString());
                }
                //*/
                #endregion

                string strStatus = strRxBuff.Substring(6, 2);

                string strBinBuff = cCmLib.hex2binary(strStatus);

                //Conveyor���°� �Ǵ� BCR ���� �ٸ� ���� Update.
                if (ScDic[m_nThNo].HEXA_SC_VALUE != strStatus)
                {
                    ScDic[m_nThNo].HEXA_SC_VALUE = strStatus; //Dictionary ���� �����Ѵ�.

                    if (strBinBuff[0] == '1')
                    {
                        ScDic[m_nThNo].START_ON = true;
                    }
                    else
                    {
                        ScDic[m_nThNo].START_ON = false;
                    }
                    string strCraneOnline = strBinBuff[0].ToString();

                    if (strBinBuff[1] == '0')
                    {
                        ScDic[m_nThNo].TRANSACTION = false;
                    }
                    else if (strBinBuff[1] == '1')
                    {
                        ScDic[m_nThNo].TRANSACTION = true;
                    }
                    string strCraneRequest = Convert.ToString("" + Convert.ToInt32(ScDic[m_nThNo].TRANSACTION));

                    if (strBinBuff[2] == '1')
                    {
                        ScDic[m_nThNo].EROOR = true;
                        m_blHostSendYN = true;
                    }
                    else 
                    {
                        ScDic[m_nThNo].EROOR = false;
                        ScDic[m_nThNo].ERR_CODE_RD = 0;
                        m_blHostSendYN = true;
                    }
                    string strIsError = strBinBuff[2].ToString();
                    string strErrorCode = Convert.ToString("" + ScDic[m_nThNo].ERR_CODE_RD);

                    if (strBinBuff[3] == '1')
                    {
                        ScDic[m_nThNo].COMEBACKHOME = true;
                    }
                    else
                    {
                        ScDic[m_nThNo].COMEBACKHOME = false;
                    }
                    string strComeBackHome = strBinBuff[3].ToString();

                    if (strBinBuff[4] == '1')
                    {
                        ScDic[m_nThNo].HOMESTOP = true;
                    }
                    else
                    {
                        ScDic[m_nThNo].HOMESTOP = false;
                    }
                    string strCraneHP = strBinBuff[4].ToString();

                    if (ScDic[m_nThNo].PROD_CHECK == true && 
                        ScDic[m_nThNo].ORDER_CHECK == false &&
                        ScDic[m_nThNo].TRANSACTION == false)
                    {
                        ScDic[m_nThNo].ORDER_CHECK = true;
                        m_blHostSendYN = true;
                    }

                    #region �ּ�.
                    ////�����ڵ尡 �ְ� ���� �����ڵ�� �ٸ�����.
                    //if (ScDic[m_nThNo].ERR_CODE_RD != nERR_CODE_RD)
                    //{
                    //    //���� �̷� �����
                    //    if (!InsertEQMT_ERR_LOG(m_strWh_typ, m_strEqmtTyp, m_strMcNo, strERR_CODE_RD, strLUGG_NO_FK1_RD))
                    //    {
                    //        return false;
                    //    }

                    //    //�����ڵ带 SET�Ѵ�.
                    //    ScDic[m_nThNo].ERR_CODE_RD = nERR_CODE_RD;
                    //    m_blHostErrSendYN = true;
                    //}
                    #endregion


                    if (!UpdateMURATA_SC_DATA(  strStatusResErrCode
                                              , strErrorCode
                                              , strStatus
                                              , strCraneOnline
                                              , strCraneRequest
                                              , strIsError
                                              , strComeBackHome
                                              , strCraneHP))
                    {
                        m_blHostSendYN = false;
                        m_blHostErrSendYN = false;
                        return false;
                    }
                }

                if (ScDic[m_nThNo].EROOR == false &&
                    ScDic[m_nThNo].START_ON == false &&
                    ScDic[m_nThNo].TRANSACTION == true)
                {
                    //StartOn();
                    if (m_msQPlc.ReadReqeust("D1", ref byRxBuff) == false)
                    {
                        //���� ��Ż��� ������Ʈ(N)
                        Communication("N", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);

                        throw new Exception();
                    }
                }

                if (Convert.ToInt32(strStatusResErrCode) != 0)
                {
                    //InitStart();
                    if (m_msQPlc.ReadReqeust("000", ref byRxBuff) == false)
                    {
                        //���� ��Ż��� ������Ʈ(N)
                        Communication("N", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);

                        throw new Exception();
                    }

                    //m_nSeqNum = 1;

                    //Sleep(500L);
                    Thread.Sleep(500);
                }

                m_blHostSendYN = false;
                m_blHostErrSendYN = false;
                //���� ��Ż��� ������Ʈ
                Communication("Y", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);
            }
            catch (Exception ex)
            {
                //EQP_MST�� CONNECTION = 'N' ������Ʈ
                Communication("N", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);
                //LOG�����
                InsertWcsLogPgr(m_strScNo, strTitle + " SC_NO : [" + m_strScNo + "] ������ �б� �� ����");

                //ȭ��ǥ��
                SetErrorMsg("Comm" + m_nThNo + strTitle + "Exception Error" + ex.Message);
                MakeMsg_Error(strTitle + m_strScNo + "Exception Error" + ex.Message, m_nThNo);

                m_blHostSendYN = false;
                m_blHostErrSendYN = false;
                return false;
            }
            return true;
        }
        /*
        private bool ScReadStatus()
        {
            string strTitle = "[ScReadStatus]";

            try
            {
                byte[] byRxBuff = new byte[2000];
                int nReadLenth = 36; //D95~130
                int nReadAddress = 95; //D96����


                MakeMsg(strTitle + "SC ���", m_nThNo);

                Array.Clear(byRxBuff, 0x00, byRxBuff.Length);
                if (m_msQPlc.READ((byte)MelsecQ3E_UnitType.MELSECQ_CMD_WORD_UNIT,
                        (byte)MelsecQ3E_UnitType_DEVICE.MELSECQ_DEVICE_CODE_D,
                        nReadAddress,
                        nReadLenth,
                        ref byRxBuff) == false)
                {
                    //���� ��Ż��� ������Ʈ(N)
                    Communication("N", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);

                    throw new Exception();
                }

                //���� ��Ż��� ������Ʈ(Y)
                Communication("Y", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);

                int nReadLen = 70;

                MakeMsg("���°� DB����", m_nThNo);

                if (!ScDic.ContainsKey(m_nThNo))
                {
                    ScDic.Add(m_nThNo, new SCData()); //Key�� �߰��Ѵ�.
                }

                //���� ���°� DIC�� �ֱ�.
                if (ScDic[m_nThNo].ONLINE_MODE_RD == "" &&
                    ScDic[m_nThNo].AUTO_MODE_RD == "" &&
                    ScDic[m_nThNo].UCSTATUS_RD == "" &&
                    ScDic[m_nThNo].ACTIVE_MODE_RD == "" &&
                    ScDic[m_nThNo].ERR_CODE_RD == 0)
                {
                    int nSelCnt = 0;

                    strSql = "";
                    strSql += CRLF = "SELECT *                   ";
                    strSql += CRLF = "  FROM SC_DATA             ";
                    strSql += CRLF = " WHERE WH_TYP = :WH_TYP    ";
                    strSql += CRLF = "   AND WH_TYP = :WH_TYP    ";
                    strSql += CRLF + "   AND PLC_NO = :PLC_NO    ";
                    strSql += CRLF + "   AND SC_NO  = :SC_NO     ";

                    m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                    m_msQPlc._pBdb.mComMain.Parameters.Clear();
                    m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                    m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                    m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;

                    nSelCnt = m_msQPlc._pBdb.ExcuteQry(strSql);

                    if (nSelCnt < 0)
                    {
                        MakeMsg_Error(strTitle + "���� SC ���� �д� �� ����(SC_DATA)", m_nThNo);
                        return false;
                    }

                    ScDic[m_nThNo].ONLINE_MODE_RD = m_msQPlc._pBdb.mDtMain.Rows[0]["ONLINE_MODE_RD"].ToString();
                    ScDic[m_nThNo].AUTO_MODE_RD = m_msQPlc._pBdb.mDtMain.Rows[0]["AUTO_MODE_RD"].ToString();
                    ScDic[m_nThNo].UCSTATUS_RD = m_msQPlc._pBdb.mDtMain.Rows[0]["UCSTATUS_RD"].ToString();
                    ScDic[m_nThNo].ACTIVE_MODE_RD = m_msQPlc._pBdb.mDtMain.Rows[0]["ACTIVE_MODE_RD"].ToString();
                    ScDic[m_nThNo].ERR_CODE_RD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[0]["ERR_CODE_RD"].ToString());
                }

                //Hexa string ������ �����´�.
                string strCvHexVal = BytesToHexs(byRxBuff, 0, nReadLen);

                //Conveyor���°� �Ǵ� BCR ���� �ٸ� ���� Update.
                if (ScDic[m_nThNo].HEXA_SC_VALUE != strCvHexVal)
                {
                    ScDic[m_nThNo].HEXA_SC_VALUE = strCvHexVal; //Dictionary ���� �����Ѵ�.

                    //D95
                    int nONLINE_MODE_RD = (byRxBuff[1] << 8) + byRxBuff[0]; //����� ���۸�� ONLINE/REMOTE
                    string strONLINE_MODE_RD = Convert.ToString("" + nONLINE_MODE_RD);
                    if (ScDic[m_nThNo].ONLINE_MODE_RD != strONLINE_MODE_RD)
                    {
                        ScDic[m_nThNo].ONLINE_MODE_RD = strONLINE_MODE_RD;
                        m_blHostSendYN = true;
                    }

                    //D96~D99 ������
                    //D96
                    int nSC_PLT_JOB_TYP_RD = (byRxBuff[3] << 8) + byRxBuff[2]; //SC PLT ����(����������)
                    string strSC_PLT_JOB_TYP_RD = Convert.ToString("" + nSC_PLT_JOB_TYP_RD);

                    //D97
                    int nD97 = (byRxBuff[5] << 8) + byRxBuff[4]; //SPARE 

                    //D98
                    int nD98 = (byRxBuff[7] << 8) + byRxBuff[6]; //SPARE

                    //D99
                    int nD99 = (byRxBuff[9] << 8) + byRxBuff[8]; //������

                    //D100
                    int nAUTO_MODE_RD = Convert.ToInt16(byRxBuff[10]);   //SC���۸��
                    string strAUTO_MODE_RD = Convert.ToString("" + nAUTO_MODE_RD);
                    if (ScDic[m_nThNo].AUTO_MODE_RD != strAUTO_MODE_RD)
                    {
                        ScDic[m_nThNo].AUTO_MODE_RD = strAUTO_MODE_RD;
                        m_blHostSendYN = true;
                    }

                    int nSENSOR_FK_RD = Convert.ToInt16(byRxBuff[11]);   //ȭ������
                    string strSENSOR_FK_RD = Convert.ToString("" + nSENSOR_FK_RD);

                    //D101
                    int nUCSTATUS_RD = (byRxBuff[13] << 8) + byRxBuff[12]; //SC���ۻ���
                    string strUCSTATUS_RD = Convert.ToString("" + nUCSTATUS_RD);
                    if (ScDic[m_nThNo].UCSTATUS_RD != strUCSTATUS_RD)
                    {
                        ScDic[m_nThNo].UCSTATUS_RD = strUCSTATUS_RD;
                        m_blHostSendYN = true;
                    }

                    //D102
                    int nPOS_H_RD = (byRxBuff[15] << 8) + byRxBuff[14]; //���� ������ġ
                    string strPOS_H_RD = Convert.ToString("" + nPOS_H_RD);

                    //D103
                    int nPOS_V_RD = (byRxBuff[17] << 8) + byRxBuff[16]; //���� �°���ġ
                    string strPOS_V_RD = Convert.ToString("" + nPOS_V_RD);

                    //D104
                    int nFORKPOS_FK1_RD = Convert.ToInt16(byRxBuff[18]); //��ũ#1 ��ġ
                    string strFORKPOS_FK1_RD = Convert.ToString("" + nFORKPOS_FK1_RD);

                    int nFORKPOS_FK2_RD = Convert.ToInt16(byRxBuff[19]); //��ũ#2 ��ġ
                    string strFORKPOS_FK2_RD = Convert.ToString("" + nFORKPOS_FK2_RD);

                    //D105
                    int nERR_CODE_RD = (byRxBuff[21] << 8) + byRxBuff[20]; //�����ڵ�
                    string strERR_CODE_RD = ((byRxBuff[21] << 8) + byRxBuff[20]).ToString("0000");

                    //D106
                    int nERR_STA_FK1_RD = Convert.ToInt16(byRxBuff[22]); //FORK#1 �����԰� ����.
                    string strERR_STA_FK1_RD = Convert.ToString("" + nERR_STA_FK1_RD);

                    int nERR_STA_FK2_RD = Convert.ToInt16(byRxBuff[23]); //FORK#1 �����԰� ����.
                    string strERR_STA_FK2_RD = Convert.ToString("" + nERR_STA_FK2_RD);

                    //int nSC_FORK2_REASS_STAT = (byRxBuff[23] >> 0) & 0x01; //FORK#2 �����߻� ������ READY.
                    //string strSC_FORK2_REASS_STAT = Convert.ToString("" + nSC_FORK2_REASS_STAT);

                    //D107 - SKI �̻��
                    //int nCV_STN_JOB_YON1 = (byRxBuff[25] << 8) + byRxBuff[24]; //CV�۾��� ��ũ�۾�����
                    //string strCV_STN_JOB_YON1 = Convert.ToString("" + nCV_STN_JOB_YON1);

                    //D108 - SKI �̻��
                    //int nCV_STN_JOB_YON2 = (byRxBuff[27] << 8) + byRxBuff[26]; //CV�۾��� ��ũ�۾�����(SPARE)
                    //string strCV_STN_JOB_YON2 = Convert.ToString("" + nCV_STN_JOB_YON2);

                    //D109
                    int nACTIVE_MODE_RD = (byRxBuff[29] << 8) + byRxBuff[28]; //���� ACTIVE ����
                    string strACTIVE_MODE_RD = Convert.ToString("" + nACTIVE_MODE_RD);
                    if (ScDic[m_nThNo].ACTIVE_MODE_RD != strACTIVE_MODE_RD)
                    {
                        ScDic[m_nThNo].ACTIVE_MODE_RD = strACTIVE_MODE_RD;
                        m_blHostSendYN = true;
                    }

                    //D110
                    int nCOMPLETE_RD = (byRxBuff[31] << 8) + byRxBuff[30]; //�۾��Ϸ�ǥ��
                    string strCOMPLETE_RD = Convert.ToString("" + nCOMPLETE_RD);

                    //D111
                    int nJOB_TYP_RD = (byRxBuff[33] << 8) + byRxBuff[32]; //SC �۾�����
                    string strJOB_TYP_RD = Convert.ToString("" + nJOB_TYP_RD);

                    //D112
                    int nLUGG_NO_FK1_RD = (byRxBuff[35] << 8) + byRxBuff[34]; //FORK1 �۾���ȣ
                    //string strLUGG_NO_FK1_RD = Convert.ToString("" + nLUGG_NO_FK1_RD);
                    string strLUGG_NO_FK1_RD = nLUGG_NO_FK1_RD.ToString("0000");

                    //D113
                    int nSTART_BANK_FK1_RD = (byRxBuff[37] << 8) + byRxBuff[36]; //FORK1 ����� ��
                    string strSTART_BANK_FK1_RD = Convert.ToString("" + nSTART_BANK_FK1_RD);

                    //D114
                    int nSTART_BAY_FK1_RD = (byRxBuff[39] << 8) + byRxBuff[38]; //FORK1 ����� ��
                    string strSTART_BAY_FK1_RD = Convert.ToString("" + nSTART_BAY_FK1_RD);

                    //D115
                    int nSTART_LEVEL_FK1_RD = (byRxBuff[41] << 8) + byRxBuff[40]; //FORK1 ����� ��
                    string strSTART_LEVEL_FK1_RD = Convert.ToString("" + nSTART_LEVEL_FK1_RD);

                    //D116
                    int nSTART_HSPOS_FK1_RD = (byRxBuff[43] << 8) + byRxBuff[42]; //FORK1 ����� �۾���
                    string strSTART_HSPOS_FK1_RD = Convert.ToString("" + nSTART_HSPOS_FK1_RD);

                    //D117
                    int nDEST_BANK_FK1_RD = (byRxBuff[45] << 8) + byRxBuff[44]; //FORK1 ������ ��
                    string strDEST_BANK_FK1_RD = Convert.ToString("" + nDEST_BANK_FK1_RD);

                    //D118
                    int nDEST_BAY_FK1_RD = (byRxBuff[47] << 8) + byRxBuff[46]; //FORK1 ������ ��
                    string strDEST_BAY_FK1_RD = Convert.ToString("" + nDEST_BAY_FK1_RD);

                    //D119
                    int nDEST_LEVEL_FK1_RD = (byRxBuff[49] << 8) + byRxBuff[48]; //FORK1 ������ ��
                    string strDEST_LEVEL_FK1_RD = Convert.ToString("" + nDEST_LEVEL_FK1_RD);

                    //D120
                    int nDEST_HSPOS_FK1_RD = (byRxBuff[51] << 8) + byRxBuff[50]; //FORK1 ������ �۾���
                    string strDEST_HSPOS_FK1_RD = Convert.ToString("" + nDEST_HSPOS_FK1_RD);

                    //D121
                    int nUSE_FK_RD = (byRxBuff[53] << 8) + byRxBuff[52]; //???
                    string strUSE_FK_RD = Convert.ToString("" + nUSE_FK_RD);

                    //D122
                    int nLUGG_NO_FK2_RD = (byRxBuff[55] << 8) + byRxBuff[54]; //FORK2 �۾���ȣ
                    //string strLUGG_NO_FK2_RD = Convert.ToString("" + nLUGG_NO_FK2_RD);
                    string strLUGG_NO_FK2_RD = nLUGG_NO_FK2_RD.ToString("0000");

                    //D123
                    int nSTART_BANK_FK2_RD = (byRxBuff[57] << 8) + byRxBuff[56]; //FORK2 ����� ��
                    string strSTART_BANK_FK2_RD = Convert.ToString("" + nSTART_BANK_FK2_RD);

                    //D124
                    int nSTART_BAY_FK2_RD = (byRxBuff[59] << 8) + byRxBuff[58]; //FORK2 ����� ��
                    string strSTART_BAY_FK2_RD = Convert.ToString("" + nSTART_BAY_FK2_RD);

                    //D125
                    int nSTART_LEVEL_FK2_RD = (byRxBuff[61] << 8) + byRxBuff[60]; //FORK2 ����� ��
                    string strSTART_LEVEL_FK2_RD = Convert.ToString("" + nSTART_LEVEL_FK2_RD);

                    //D126
                    int nSTART_HSPOS_FK2_RD = (byRxBuff[63] << 8) + byRxBuff[62]; //FORK2 ����� �۾���
                    string strSTART_HSPOS_FK2_RD = Convert.ToString("" + nSTART_HSPOS_FK2_RD);

                    //D127
                    int nDEST_BANK_FK2_RD = (byRxBuff[65] << 8) + byRxBuff[64]; //FORK2 ������ ��
                    string strDEST_BANK_FK2_RD = Convert.ToString("" + nDEST_BANK_FK2_RD);

                    //D128
                    int nDEST_HSPOS_FK2_RD = (byRxBuff[67] << 8) + byRxBuff[66]; //FORK2 ������ �۾���
                    string strDEST_HSPOS_FK2_RD = Convert.ToString("" + nDEST_HSPOS_FK2_RD);

                    //D129
                    int nDEST_BAY_FK2_RD = (byRxBuff[69] << 8) + byRxBuff[68]; //FORK2 ������ ��
                    string strDEST_BAY_FK2_RD = Convert.ToString("" + nDEST_BAY_FK2_RD);

                    //D130
                    int nDEST_LEVEL_FK2_RD = (byRxBuff[71] << 8) + byRxBuff[70]; //FORK2 ������ ��
                    string strDEST_LEVEL_FK2_RD = Convert.ToString("" + nDEST_LEVEL_FK2_RD);

                    //�����ڵ尡 �ְ� ���� �����ڵ�� �ٸ�����.
                    if (ScDic[m_nThNo].ERR_CODE_RD != nERR_CODE_RD)
                    {
                        //���� �̷� �����
                        if (!InsertEQMT_ERR_LOG(m_strWh_typ, m_strEqmtTyp, m_strMcNo, strERR_CODE_RD, strLUGG_NO_FK1_RD))
                        {
                            return false;
                        }

                        //�����ڵ带 SET�Ѵ�.
                        ScDic[m_nThNo].ERR_CODE_RD = nERR_CODE_RD;
                        m_blHostErrSendYN = true;
                    }

                    //TRACK���� UPDATE.

                    if (!UpdateSC_DATA(strONLINE_MODE_RD
                                       , strAUTO_MODE_RD
                                       , strSENSOR_FK_RD
                                       , strUCSTATUS_RD
                                       , strPOS_H_RD
                                       , strPOS_V_RD
                                       , strFORKPOS_FK1_RD
                                       , strFORKPOS_FK2_RD
                                       , strERR_CODE_RD
                                       , strERR_STA_FK1_RD
                                       , strERR_STA_FK2_RD
                                       , strACTIVE_MODE_RD
                                       , strCOMPLETE_RD
                                       , strJOB_TYP_RD
                                       , strLUGG_NO_FK1_RD
                                       , strSTART_BANK_FK1_RD
                                       , strSTART_HSPOS_FK1_RD
                                       , strDEST_BANK_FK1_RD
                                       , strDEST_HSPOS_FK1_RD
                                       , strUSE_FK_RD
                                       , strLUGG_NO_FK2_RD
                                       , strSTART_BANK_FK2_RD
                                       , strSTART_HSPOS_FK2_RD
                                       , strDEST_BANK_FK2_RD
                                       , strDEST_HSPOS_FK2_RD
                                       , strSTART_BAY_FK1_RD
                                       , strSTART_LEVEL_FK1_RD
                                       , strSTART_BAY_FK2_RD
                                       , strSTART_LEVEL_FK2_RD
                                       , strDEST_BAY_FK1_RD
                                       , strDEST_LEVEL_FK1_RD
                                       , strDEST_BAY_FK2_RD
                                       , strDEST_LEVEL_FK2_RD
                                       , strSC_PLT_JOB_TYP_RD))
                    {
                        m_blHostSendYN = false;
                        m_blHostErrSendYN = false;
                        return false;
                    }
                }
                m_blHostSendYN = false;
                m_blHostErrSendYN = false;
                //���� ��Ż��� ������Ʈ
                Communication("Y", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);
            }
            catch (Exception ex)
            {
                //EQP_MST�� CONNECTION = 'N' ������Ʈ
                Communication("N", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);
                //LOG�����
                InsertWcsLogPgr(m_strScNo, strTitle + " SC_NO : [" + m_strScNo + "] ������ �б� �� ����");

                //ȭ��ǥ��
                SetErrorMsg("Comm" + m_nThNo + strTitle + "Exception Error" + ex.Message);
                MakeMsg_Error(strTitle + m_strScNo + "Exception Error" + ex.Message, m_nThNo);

                m_blHostSendYN = false;
                m_blHostErrSendYN = false;
                return false;
            }
            return true;
        }
        //*/
        #endregion

        #region [SC_CMD_RQ_YN] :: SC_DATA���� CMD_RQ_YN = 'Y'�ΰ� ã�Ƽ� CMD �� ����
        private bool SC_CMD_RQ_YN()
        {
            string strTitle = "[SC_CMD_RQ_YN]";

            try
            {
                byte[] byTxBuff = new byte[1000];

                int nUpdCount = 0;

                //��û ��ȸ
                strSql = "";
                strSql += CRLF + "SELECT SD.*                                          ";
                strSql += CRLF + "  FROM SC_DATA SD                                    ";
                strSql += CRLF + " WHERE SD.WH_TYP = :WH_TYP                           ";
                strSql += CRLF + "   AND SD.PLC_NO = :PLC_NO                           ";
                strSql += CRLF + "   AND SD.SC_NO  = :SC_NO                            ";
                strSql += CRLF + "   AND SD.CMD_RQ_YN = 'Y'                            ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;

                nUpdCount = m_msQPlc._pBdb.ExcuteQry(strSql);

                if (nUpdCount < 0)
                {
                    MakeMsg_Error(strTitle + "SC���� �д� �� ����(SC_DATA)", m_nThNo);
                    return false;
                }


                for (int nRows = 0; nRows < nUpdCount; nRows++)
                {
                    string strWriteMsg = "";

                    //��������
                    string strCMD_RQ_ID = Convert.ToString("" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["CMD_RQ_ID"]);


                    int nODVal = 0;
                    int nWriteLen = 0;
                    int nWriteAddr = 0;
                    Array.Clear(byTxBuff, 0, byTxBuff.Length);

                    switch (strCMD_RQ_ID)
                    {
                        //�������.
                        case "EMERGENCY":
                            nODVal = 1;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);

                            strWriteMsg = "EMERGENCY ���";
                            break;

                        //ACTIVE
                        case "ACTIVE":
                            nODVal = 2;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);

                            strWriteMsg = "ACTIVE ���";
                            break;

                        //PAUSE
                        case "PAUSE":
                            nODVal = 4;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);

                            strWriteMsg = "PAUSE ���";
                            break;

                        //ERROR RESET
                        case "RESET":
                            nODVal = 8;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);

                            strWriteMsg = "ERROR RESET ���";
                            break;

                        //FORK DATA ����.
                        case "DELFK1":
                            //1�� ��ũ��
                            nODVal = 16;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            strWriteMsg = "FORK#1 DATA���� ���";
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            break;

                        case "DELFK2":
                            //2�� ��ũ��
                            nODVal = 32;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            strWriteMsg = "FORK#2 DATA���� ���";
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            break;

                        case "DELFK12":
                            //���� ��ũ��
                            nODVal = 64;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            strWriteMsg = "FORK#1,2 DATA���� ���";
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            break;

                        //����� ONLINE
                        case "ONL":
                            nODVal = 128;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            strWriteMsg = "����� ONLINE ���";
                            break;

                        //����� OFFLINE
                        case "OFL":
                            nODVal = 256;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            strWriteMsg = "����� OFFLINE ���";
                            break;

                        //����� REMOTE
                        case "REM":
                            nODVal = 512;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            strWriteMsg = "����� REMOTE ���";
                            break;

                        //����� ERROR
                        case "GRE":
                            nODVal = 1024;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            strWriteMsg = "����� ERROR ���";
                            break;
                        case "FCMP":
                            //���α� �����
                            m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "] CMD_RQ_ID : [" + strCMD_RQ_ID + "] Ŭ���̾�Ʈ ��� ����.";
                            if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                            {
                                return false;
                            }
                            return true;
                        default:
                            //���α� �����
                            m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "] CMD_RQ_ID : [" + strCMD_RQ_ID + "] �����ǵ� CMD.";
                            if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                            {
                                return false;
                            }
                            return true;
                    }

                    //�� ���� �� �޼��� ǥ��.
                    MakeMsg_Imp(strTitle + strWriteMsg, m_nThNo);

                    if (m_msQPlc.WRITE((byte)MelsecQ3E_UnitType.MELSECQ_CMD_WORD_UNIT,
                                                       (byte)MelsecQ3E_UnitType_DEVICE.MELSECQ_DEVICE_CODE_D,
                                                       nWriteAddr,
                                                       nWriteLen,
                                                       byTxBuff) == false)
                    {
                        if (this.m_msQPlc.IsHex)
                        {
                            MakeMsg_Error(strTitle + "SC���������û SEND [" + m_msQPlc.SndHexString + "]", m_nThNo);
                            MakeMsg_Error(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvHexString + "]", m_nThNo);
                        }
                        if (this.m_msQPlc.IsAscii)
                        {
                            MakeMsg_Error(strTitle + "SC���������û SEND [" + m_msQPlc.SndAsciiString + "]", m_nThNo);
                            MakeMsg_Error(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvAsciiString + "]", m_nThNo);
                        }

                        m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "] CMD_RQ_ID : [" + strCMD_RQ_ID + "] Ŀ�ǵ� ���� ����";
                        if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                        {
                            return false;
                        }

                        return false;
                    }
                    if (this.m_msQPlc.IsHex)
                    {
                        MakeMsg_Imp(strTitle + "SC���������û SEND [" + m_msQPlc.SndHexString + "]", m_nThNo);
                        MakeMsg_Imp(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvHexString + "]", m_nThNo);
                    }
                    if (this.m_msQPlc.IsAscii)
                    {
                        MakeMsg_Imp(strTitle + "SC���������û SEND [" + m_msQPlc.SndAsciiString + "]", m_nThNo);
                        MakeMsg_Imp(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvAsciiString + "]", m_nThNo);
                    }

                    //���� �α� �����
                    m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "] CMD_RQ_ID : [" + strCMD_RQ_ID + "] Ŀ�ǵ� ���� ����";
                    if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                    {
                        return false;
                    }


                    //CMD_RQ_YN = 'N' ������Ʈ
                    if (!UpdateSC_CMD_RQ_YN(strCMD_RQ_ID))
                    {
                        return false;
                    }


                }

                return true;
            }
            catch (Exception ex)
            {
                string msg = strTitle + " " + ex.Message;
                SetErrorMsg("Comm" + m_nThNo + ex.Message);
                MakeMsg_Error(ex.Message, m_nThNo);
                InsertWcsLogPgr(m_strScNo, msg);
                return false;
            }
        }
        #endregion

        #region [MURATA_SC_CMD_RQ_YN] :: SC_DATA_MURATA���� CMD_RQ_YN = 'Y'�ΰ� ã�Ƽ� CMD �� ����

        // ����Ÿ������ ��ȭ���ڸ� ���ؼ� �۾������ϴ� ��찡 ���� ����!
        // ������ �ϴµ� �� ���� IO_SCH���� �ؾ��� �۾����� C++������ ������ ����!!!
        // SC_INFO->m_bInvoke = FALSE;          SC_INFO->m_nLuggNum = 0;        
        // �׸��� �α� ��� 
        private bool MURATA_SC_CMD_RQ_YN()
        {
            string strTitle = "[MURATA_SC_CMD_RQ_YN]";

            try
            {
                byte[] byTxBuff = new byte[1000];

                int nUpdCount = 0;

                //��û ��ȸ
                strSql = "";
                strSql += CRLF + "SELECT SD.*                                          ";
                strSql += CRLF + "  FROM SC_DATA_MURATA SD                             ";
                strSql += CRLF + " WHERE SD.WH_TYP = :WH_TYP                           ";
                strSql += CRLF + "   AND SD.PLC_NO = :PLC_NO                           ";
                strSql += CRLF + "   AND SD.SC_NO  = :SC_NO                            ";
                strSql += CRLF + "   AND SD.CMD_RQ_YN = 'Y'                            ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;

                nUpdCount = m_msQPlc._pBdb.ExcuteQry(strSql);

                if (nUpdCount < 0)
                {
                    MakeMsg_Error(strTitle + "SC���� �д� �� ����(SC_DATA_MURATA)", m_nThNo);
                    return false;
                }


                for (int nRows = 0; nRows < nUpdCount; nRows++)
                {
                    string strWriteMsg = "";
                    string strCmdId = "";

                    //��������
                    string strCMD_RQ_ID = Convert.ToString("" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["CMD_RQ_ID"]);


                    int nODVal = 0;
                    int nWriteLen = 0;
                    int nWriteAddr = 0;
                    Array.Clear(byTxBuff, 0, byTxBuff.Length);

                    switch (strCMD_RQ_ID)
                    {
                        //�������.
                        case "EMERGENCY":
                            //nODVal = 1;
                            //nWriteLen = 1;
                            //nWriteAddr = 199;
                            //byTxBuff[0] = (byte)(nODVal >> 0);
                            //byTxBuff[1] = (byte)(nODVal >> 8);

                            strWriteMsg = "EMERGENCY ���";
                            strCmdId = "999";

                            break;

                        ////ACTIVE
                        //case "ACTIVE":
                        //    nODVal = 2;
                        //    nWriteLen = 1;
                        //    nWriteAddr = 199;
                        //    byTxBuff[0] = (byte)(nODVal >> 0);
                        //    byTxBuff[1] = (byte)(nODVal >> 8);

                        //    strWriteMsg = "ACTIVE ���";
                        //    break;

                        ////PAUSE
                        //case "PAUSE":
                        //    nODVal = 4;
                        //    nWriteLen = 1;
                        //    nWriteAddr = 199;
                        //    byTxBuff[0] = (byte)(nODVal >> 0);
                        //    byTxBuff[1] = (byte)(nODVal >> 8);

                        //    strWriteMsg = "PAUSE ���";
                        //    break;

                        //ERROR RESET
                        case "RESET":
                            //nODVal = 8;
                            //nWriteLen = 1;
                            //nWriteAddr = 199;
                            //byTxBuff[0] = (byte)(nODVal >> 0);
                            //byTxBuff[1] = (byte)(nODVal >> 8);

                            strWriteMsg = "ERROR RESET ���";
                            //strCmdId = "999";
                            return true;
                            //break;

                        //FORK DATA ����.
                        case "DELFK1":
                            //1�� ��ũ��
                            nODVal = 16;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            strWriteMsg = "FORK#1 DATA���� ���";
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            break;

                        case "DELFK2":
                            //2�� ��ũ��
                            nODVal = 32;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            strWriteMsg = "FORK#2 DATA���� ���";
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            break;

                        case "DELFK12":
                            //���� ��ũ��
                            nODVal = 64;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            strWriteMsg = "FORK#1,2 DATA���� ���";
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            break;

                        //����� ONLINE
                        case "ONL":
                            nODVal = 128;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            strWriteMsg = "����� ONLINE ���";
                            break;

                        //����� OFFLINE
                        case "OFL":
                            nODVal = 256;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            strWriteMsg = "����� OFFLINE ���";
                            break;

                        //����� REMOTE
                        case "REM":
                            nODVal = 512;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            strWriteMsg = "����� REMOTE ���";
                            break;

                        //����� ERROR
                        case "GRE":
                            nODVal = 1024;
                            nWriteLen = 1;
                            nWriteAddr = 199;
                            byTxBuff[0] = (byte)(nODVal >> 0);
                            byTxBuff[1] = (byte)(nODVal >> 8);
                            strWriteMsg = "����� ERROR ���";
                            break;
                        case "FCMP":
                            //���α� �����
                            m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "] CMD_RQ_ID : [" + strCMD_RQ_ID + "] Ŭ���̾�Ʈ ��� ����.";
                            if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                            {
                                return false;
                            }
                            return true;
                        default:
                            //���α� �����
                            m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "] CMD_RQ_ID : [" + strCMD_RQ_ID + "] �����ǵ� CMD.";
                            if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                            {
                                return false;
                            }
                            return true;
                    }

                    // strCmd : StatusRead => "A1", ErrorRead => "A2",
                    // strCmd : Move => "B1", Store => "B2", Retrieve => "B3", CellToCell => "B4",
                    // strCmd : DestChange(Cell) => "B5", DestChange(ST) => "B6"
                    // strCmd : StartOn => "D1", Clear => "D2", ReturnHP => "D3", ErrorReset => "D4"
                    if (m_msQPlc.ReadReqeust(strCmdId, ref byTxBuff) == false)
                    {
                        //���� ��Ż��� ������Ʈ(N)
                        Communication("N", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);

                        throw new Exception();
                    }


                    /*
                    //�� ���� �� �޼��� ǥ��.
                    MakeMsg_Imp(strTitle + strWriteMsg, m_nThNo);

                    if (m_msQPlc.WRITE((byte)MelsecQ3E_UnitType.MELSECQ_CMD_WORD_UNIT,
                                                       (byte)MelsecQ3E_UnitType_DEVICE.MELSECQ_DEVICE_CODE_D,
                                                       nWriteAddr,
                                                       nWriteLen,
                                                       byTxBuff) == false)
                    {
                        if (this.m_msQPlc.IsHex)
                        {
                            MakeMsg_Error(strTitle + "SC���������û SEND [" + m_msQPlc.SndHexString + "]", m_nThNo);
                            MakeMsg_Error(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvHexString + "]", m_nThNo);
                        }
                        if (this.m_msQPlc.IsAscii)
                        {
                            MakeMsg_Error(strTitle + "SC���������û SEND [" + m_msQPlc.SndAsciiString + "]", m_nThNo);
                            MakeMsg_Error(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvAsciiString + "]", m_nThNo);
                        }

                        m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "] CMD_RQ_ID : [" + strCMD_RQ_ID + "] Ŀ�ǵ� ���� ����";
                        if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                        {
                            return false;
                        }

                        return false;
                    }
                    if (this.m_msQPlc.IsHex)
                    {
                        MakeMsg_Imp(strTitle + "SC���������û SEND [" + m_msQPlc.SndHexString + "]", m_nThNo);
                        MakeMsg_Imp(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvHexString + "]", m_nThNo);
                    }
                    if (this.m_msQPlc.IsAscii)
                    {
                        MakeMsg_Imp(strTitle + "SC���������û SEND [" + m_msQPlc.SndAsciiString + "]", m_nThNo);
                        MakeMsg_Imp(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvAsciiString + "]", m_nThNo);
                    }

                    //���� �α� �����
                    m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "] CMD_RQ_ID : [" + strCMD_RQ_ID + "] Ŀ�ǵ� ���� ����";
                    if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                    {
                        return false;
                    }
                    //*/

                    //CMD_RQ_YN = 'N' ������Ʈ
                    if (!UpdateSC_CMD_RQ_YN(strCMD_RQ_ID))
                    {
                        return false;
                    }


                }

                return true;
            }
            catch (Exception ex)
            {
                string msg = strTitle + " " + ex.Message;
                SetErrorMsg("Comm" + m_nThNo + ex.Message);
                MakeMsg_Error(ex.Message, m_nThNo);
                InsertWcsLogPgr(m_strScNo, msg);
                return false;
            }
        }
        #endregion

        #region [SC_OD_RQ_YN] :: SC_DATA���� OD_RQ_YN = 'Y'�ΰ� ã�Ƽ� SC ����
        public bool SC_OD_RQ_YN()
        {
            string strTitle = "[SC_OD_RQ_YN]";

            try
            {
                byte[] byTxBuff = new byte[1000];

                int nUpdCount = 0;
                string strWriteMsg = "";

                //��û ��ȸ
                strSql = "";
                strSql += CRLF + "SELECT SD.*                                          ";
                strSql += CRLF + "  FROM SC_DATA SD                                    ";
                strSql += CRLF + " WHERE SD.WH_TYP = :WH_TYP                           ";
                strSql += CRLF + "   AND SD.PLC_NO = :PLC_NO                           ";
                strSql += CRLF + "   AND SD.SC_NO  = :SC_NO                            ";
                strSql += CRLF + "   AND SD.OD_RQ_YN = 'Y'                             ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;


                nUpdCount = m_msQPlc._pBdb.ExcuteQry(strSql);

                if (nUpdCount < 0)
                {
                    MakeMsg_Error(strTitle + "SC ���� �д� �� ����(SC_DATA)", m_nThNo);
                    return false;
                }


                for (int nRows = 0; nRows < nUpdCount; nRows++)
                {
                    int n_JOB_TYP_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["JOB_TYP_OD"].ToString());
                    int n_LUGG_NO_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["LUGG_NO_FK1_OD"].ToString());
                    string str_LUGG_NO_FK1_OD = n_LUGG_NO_FK1_OD.ToString("0000");
                    int n_START_BANK_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_BANK_FK1_OD"].ToString());
                    int n_START_BAY_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_BAY_FK1_OD"].ToString());
                    int n_START_LEVEL_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_LEVEL_FK1_OD"].ToString());
                    int n_START_HSPOS_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_HSPOS_FK1_OD"].ToString());
                    int n_DEST_BANK_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_BANK_FK1_OD"].ToString());
                    int n_DEST_BAY_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_BAY_FK1_OD"].ToString());
                    int n_DEST_LEVEL_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_LEVEL_FK1_OD"].ToString());
                    int n_DEST_HSPOS_FK1_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_HSPOS_FK1_OD"].ToString());
                    int n_USE_FK_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["USE_FK_OD"].ToString());
                    int n_LUGG_NO_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["LUGG_NO_FK2_OD"].ToString());
                    string str_LUGG_NO_FK2_OD = n_LUGG_NO_FK2_OD.ToString("0000");
                    int n_START_BANK_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_BANK_FK2_OD"].ToString());
                    int n_START_BAY_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_BAY_FK2_OD"].ToString());
                    int n_START_LEVEL_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_LEVEL_FK2_OD"].ToString());
                    int n_START_HSPOS_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_HSPOS_FK2_OD"].ToString());
                    int n_DEST_BANK_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_BANK_FK2_OD"].ToString());
                    int n_DEST_BAY_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_BAY_FK2_OD"].ToString());
                    int n_DEST_LEVEL_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_LEVEL_FK2_OD"].ToString());
                    int n_DEST_HSPOS_FK2_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_HSPOS_FK2_OD"].ToString());
                    int n_SC_FIRE_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["SC_FIRE_OD"].ToString()); //�߱� SKI���
                    int n_SC_PLT_JOB_TYP_OD = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["SC_PLT_JOB_TYP_OD"].ToString()); //


                    strWriteMsg = "FORK#1,2 �۾����� ";

                    int nWriteLen = 29;
                    int nWriteAddr = 171;

                    //�۾�����[D171][01:�԰�, 02:���, 03:�����, 04:���ġ, 05:Ȩ����][
                    byTxBuff[0] = (byte)(n_JOB_TYP_OD >> 0);
                    byTxBuff[1] = (byte)(n_JOB_TYP_OD >> 8);
                    strWriteMsg += "SC_JOB_TYP [" + n_JOB_TYP_OD + "] ";

                    //��ũ#1�۾���ȣ[D172]
                    byTxBuff[2] = (byte)(n_LUGG_NO_FK1_OD >> 0);
                    byTxBuff[3] = (byte)(n_LUGG_NO_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_LUG_NO [" + n_LUGG_NO_FK1_OD + "] ";

                    //��ũ#1����� ��(Bank)[D173]
                    byTxBuff[4] = (byte)(n_START_BANK_FK1_OD >> 0);
                    byTxBuff[5] = (byte)(n_START_BANK_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_FROM_BANK [" + n_START_BANK_FK1_OD + "] ";

                    //��ũ#1����� ��(Bay)[D174]
                    byTxBuff[6] = (byte)(n_START_BAY_FK1_OD >> 0);
                    byTxBuff[7] = (byte)(n_START_BAY_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_FROM_BAY [" + n_START_BAY_FK1_OD + "] ";

                    //��ũ#1����� ��(Level)[D175]
                    byTxBuff[8] = (byte)(n_START_LEVEL_FK1_OD >> 0);
                    byTxBuff[9] = (byte)(n_START_LEVEL_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_FROM_LEV [" + n_START_LEVEL_FK1_OD + "] ";

                    //��ũ#1����� �۾���[D176]
                    byTxBuff[10] = (byte)(n_START_HSPOS_FK1_OD >> 0);
                    byTxBuff[11] = (byte)(n_START_HSPOS_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_FROM_SITE [" + n_START_HSPOS_FK1_OD + "] ";

                    //��ũ#1������ ��(Bank)[D177]
                    byTxBuff[12] = (byte)(n_DEST_BANK_FK1_OD >> 0);
                    byTxBuff[13] = (byte)(n_DEST_BANK_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_TO_BANK [" + n_DEST_BANK_FK1_OD + "] ";

                    //��ũ#1������ ��(Bay)[D178]
                    byTxBuff[14] = (byte)(n_DEST_BAY_FK1_OD >> 0);
                    byTxBuff[15] = (byte)(n_DEST_BAY_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_TO_BAY [" + n_DEST_BAY_FK1_OD + "] ";

                    //��ũ#1������ ��(Level)[D179]
                    byTxBuff[16] = (byte)(n_DEST_LEVEL_FK1_OD >> 0);
                    byTxBuff[17] = (byte)(n_DEST_LEVEL_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_TO_LEV [" + n_DEST_LEVEL_FK1_OD + "] ";

                    //��ũ#1������ �۾���[D180]
                    byTxBuff[18] = (byte)(n_DEST_HSPOS_FK1_OD >> 0);
                    byTxBuff[19] = (byte)(n_DEST_HSPOS_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_TO_SITE [" + n_DEST_HSPOS_FK1_OD + "] ";

                    //��ũ��� ����[D181] [0:��ũ#1�� ���, 1:��ũ#1~��ũ#2 ���û��, 2:��ũ#2�� ��� ]
                    byTxBuff[20] = (byte)(n_USE_FK_OD >> 0);
                    byTxBuff[21] = (byte)(n_USE_FK_OD >> 8);
                    strWriteMsg += "FORK��뱸�� [1] ";

                    //��ũ#2�۾���ȣ[D182]
                    byTxBuff[22] = (byte)(n_LUGG_NO_FK2_OD >> 0);
                    byTxBuff[23] = (byte)(n_LUGG_NO_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_LUG_NO [" + n_LUGG_NO_FK2_OD + "] ";

                    //��ũ#2����� ��(Bank)[D183]
                    byTxBuff[24] = (byte)(n_START_BANK_FK2_OD >> 0);
                    byTxBuff[25] = (byte)(n_START_BANK_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_FROM_BANK [" + n_START_BANK_FK2_OD + "] ";

                    //��ũ#2����� ��(Bay)[D184]
                    byTxBuff[26] = (byte)(n_START_BAY_FK2_OD >> 0);
                    byTxBuff[27] = (byte)(n_START_BAY_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_FROM_BAY [" + n_START_BAY_FK2_OD + "] ";

                    //��ũ#2����� ��(Level)[D185]
                    byTxBuff[28] = (byte)(n_START_LEVEL_FK2_OD >> 0);
                    byTxBuff[29] = (byte)(n_START_LEVEL_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_FROM_LEV [" + n_START_LEVEL_FK2_OD + "] ";

                    //��ũ#2����� �۾���[D186]
                    byTxBuff[30] = (byte)(n_START_HSPOS_FK2_OD >> 0);
                    byTxBuff[31] = (byte)(n_START_HSPOS_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_FROM_SITE [" + n_START_HSPOS_FK2_OD + "] ";

                    //��ũ#2������ ��(Bank)[D187]
                    byTxBuff[32] = (byte)(n_DEST_BANK_FK2_OD >> 0);
                    byTxBuff[33] = (byte)(n_DEST_BANK_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_TO_BANK [" + n_DEST_BANK_FK2_OD + "] ";

                    //��ũ#2������ ��(Bay)[D188]
                    byTxBuff[34] = (byte)(n_DEST_BAY_FK2_OD >> 0);
                    byTxBuff[35] = (byte)(n_DEST_BAY_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_TO_BAY [" + n_DEST_BAY_FK2_OD + "] ";

                    //��ũ#2������ ��(Level)[D189]
                    byTxBuff[36] = (byte)(n_DEST_LEVEL_FK2_OD >> 0);
                    byTxBuff[37] = (byte)(n_DEST_LEVEL_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_TO_LEV [" + n_DEST_LEVEL_FK2_OD + "] ";

                    //��ũ#2������ �۾���[D190]
                    byTxBuff[38] = (byte)(n_DEST_HSPOS_FK2_OD >> 0);
                    byTxBuff[39] = (byte)(n_DEST_HSPOS_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_TO_SITE [" + n_DEST_HSPOS_FK2_OD + "] ";

                    //�۾�����ƮWrite Flag[D191][1�� ���� ���ݿ��� 0���� ����] [D191]
                    byTxBuff[40] = (byte)(1 >> 0);
                    byTxBuff[41] = (byte)(1 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [1] ";

                    //[D192]
                    byTxBuff[42] = (byte)(n_SC_PLT_JOB_TYP_OD >> 0);
                    byTxBuff[43] = (byte)(n_SC_PLT_JOB_TYP_OD >> 8);
                    strWriteMsg += "SC_PLT_JOB_TYP_OD [" + n_SC_PLT_JOB_TYP_OD + "] ";

                    //[D193]
                    byTxBuff[44] = (byte)(0 >> 0);
                    byTxBuff[45] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D194]
                    byTxBuff[46] = (byte)(0 >> 0);
                    byTxBuff[47] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D195]
                    byTxBuff[48] = (byte)(0 >> 0);
                    byTxBuff[49] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D196]
                    byTxBuff[50] = (byte)(0 >> 0);
                    byTxBuff[51] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D197]
                    byTxBuff[52] = (byte)(0 >> 0);
                    byTxBuff[53] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D198]
                    byTxBuff[54] = (byte)(0 >> 0);
                    byTxBuff[55] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D199]
                    byTxBuff[56] = (byte)(0 >> 0);
                    byTxBuff[57] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //�� ���� �� �޼��� ǥ��.
                    MakeMsg_Imp(strTitle + strWriteMsg, m_nThNo);

                    if (m_msQPlc.WRITE((byte)MelsecQ3E_UnitType.MELSECQ_CMD_WORD_UNIT,
                                                       (byte)MelsecQ3E_UnitType_DEVICE.MELSECQ_DEVICE_CODE_D,
                                                       nWriteAddr,
                                                       nWriteLen,
                                                       byTxBuff) == false)
                    {
                        if (this.m_msQPlc.IsHex)
                        {
                            MakeMsg_Error(strTitle + "SC���������û SEND [" + m_msQPlc.SndHexString + "]", m_nThNo);
                            MakeMsg_Error(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvHexString + "]", m_nThNo);
                        }
                        if (this.m_msQPlc.IsAscii)
                        {
                            MakeMsg_Error(strTitle + "SC���������û SEND [" + m_msQPlc.SndAsciiString + "]", m_nThNo);
                            MakeMsg_Error(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvAsciiString + "]", m_nThNo);
                        }

                        m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "], �۾���ȣ : [" + str_LUGG_NO_FK1_OD + "], �۾����� : [" + Convert.ToString("" + n_JOB_TYP_OD) + "], SC ���� ����";
                        if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                        {
                            return false;
                        }

                        return false;
                    }
                    if (this.m_msQPlc.IsHex)
                    {
                        MakeMsg_Imp(strTitle + "SC���������û SEND [" + m_msQPlc.SndHexString + "]", m_nThNo);
                        MakeMsg_Imp(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvHexString + "]", m_nThNo);
                    }
                    if (this.m_msQPlc.IsAscii)
                    {
                        MakeMsg_Imp(strTitle + "SC���������û SEND [" + m_msQPlc.SndAsciiString + "]", m_nThNo);
                        MakeMsg_Imp(strTitle + "SC���������û RECEIVE [" + m_msQPlc.RcvAsciiString + "]", m_nThNo);
                    }

                    //���� �α� �����
                    m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "], �۾���ȣ : [" + str_LUGG_NO_FK1_OD + "], �۾����� : [" + Convert.ToString("" + n_JOB_TYP_OD) + "], SC ���� ����";
                    if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                    {
                        return false;
                    }

                    //OD_RQ_YN = 'N' ������Ʈ
                    if (!UpdateSC_OD_RQ_YN(str_LUGG_NO_FK1_OD, "0"))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string msg = strTitle + " " + ex.Message;
                SetErrorMsg("Comm" + m_nThNo + ex.Message);
                MakeMsg_Error(ex.Message, m_nThNo);
                InsertWcsLogPgr(m_strScNo, msg);
                return false;
            }
        }
        #endregion

        #region [MURATA_SC_OD_RQ_YN] :: SC_DATA_MURATA���� OD_RQ_YN = 'Y'�ΰ� ã�Ƽ� SC ����
        public bool MURATA_SC_OD_RQ_YN()
        {
            string strTitle = "[MURATA_SC_OD_RQ_YN]";

            try
            {
                byte[] byTxBuff = new byte[1000];

                int nUpdCount = 0;
                string strWriteMsg = "";

                //��û ��ȸ
                strSql = "";
                strSql += CRLF + "SELECT SD.*                                          ";
                strSql += CRLF + "  FROM SC_DATA_MURATA SD                             ";
                strSql += CRLF + " WHERE SD.WH_TYP = :WH_TYP                           ";
                strSql += CRLF + "   AND SD.PLC_NO = :PLC_NO                           ";
                strSql += CRLF + "   AND SD.SC_NO  = :SC_NO                            ";
                strSql += CRLF + "   AND SD.OD_RQ_YN = 'Y'                             ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;


                nUpdCount = m_msQPlc._pBdb.ExcuteQry(strSql);

                if (nUpdCount < 0)
                {
                    MakeMsg_Error(strTitle + "SC ���� �д� �� ����(SC_DATA)", m_nThNo);
                    return false;
                }


                for (int nRows = 0; nRows < nUpdCount; nRows++)
                {
                    string str_JOB_TYP = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["JOB_TYP"].ToString();
                    int n_LUGG_NO = Convert.ToInt32(0 + m_msQPlc._pBdb.mDtMain.Rows[nRows]["LUGG_NO"].ToString());
                    string str_LUGG_NO          = n_LUGG_NO.ToString("0000");
                    string str_START_BANK       = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_BANK"].ToString();
                    string str_START_BAY        = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_BAY"].ToString();
                    string str_START_LEVEL      = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_LEVEL"].ToString();
                    string str_START_HSPOS      = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_HSPOS"].ToString();
                    string str_SATRT_DEEP_CELL  = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["START_DEEP_CELL"].ToString();
                    string str_DEST_BANK        = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_BANK"].ToString();
                    string str_DEST_BAY         = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_BAY"].ToString();
                    string str_DEST_LEVEL       = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_LEVEL"].ToString();
                    string str_DEST_HSPOS       = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_HSPOS"].ToString();
                    string str_DEST_DEEP_CELL   = "" + m_msQPlc._pBdb.mDtMain.Rows[nRows]["DEST_DEEP_CELL"].ToString();

                    strWriteMsg = m_msQPlc._pBdb.mDtMain.Rows[nRows]["MC_NO_NM"].ToString() + "�۾����� ";

                    int nDestOffSet = 0;

                    // �۾�����(B1, B2...)
                    //nCopyCnt = str_JOB_TYP.Length;       Buffer.BlockCopy(Encoding.UTF8.GetBytes(str_JOB_TYP), 0, byTxBuff, nDestOffSet, nCopyCnt);          nDestOffSet += nCopyCnt;
                    cCmLib.FieldStrCopyToByteArray(str_JOB_TYP, "JOB_TYP", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);                 // �۾�����
                    cCmLib.FieldStrCopyToByteArray("1", "FORK", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);                            // ��� FORK
                    cCmLib.FieldStrCopyToByteArray(str_START_HSPOS, "L/S", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);                 // ��� H/S
                    cCmLib.FieldStrCopyToByteArray(str_START_BANK, "BANK", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);                 // ��� BAMK
                    cCmLib.FieldStrCopyToByteArray(str_START_BAY, "BAY", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);                   // ��� BAY
                    cCmLib.FieldStrCopyToByteArray(str_START_LEVEL, "LEVEL", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);               // ��� LEVEL
                    cCmLib.FieldStrCopyToByteArray(str_SATRT_DEEP_CELL, "DEEP_CEEL", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);       // ��� DEEP CELL
                    if (str_JOB_TYP == "B4")
                    {
                        cCmLib.FieldStrCopyToByteArray("1", "FORK", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);                        // ���� FORK
                        cCmLib.FieldStrCopyToByteArray(str_DEST_HSPOS, "L/S", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);              // ���� H/S
                        cCmLib.FieldStrCopyToByteArray(str_DEST_BANK, "BANK", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);              // ���� BAMK
                        cCmLib.FieldStrCopyToByteArray(str_DEST_BAY, "BAY", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);                // ���� BAY
                        cCmLib.FieldStrCopyToByteArray(str_DEST_LEVEL, "LEVEL", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);            // ���� LEVEL
                        cCmLib.FieldStrCopyToByteArray(str_DEST_DEEP_CELL, "DEEP_CELL", ref byTxBuff, ref nDestOffSet, ref strWriteMsg);    // ���� DEEP CELL
                    }

                    #region ���� ����(�ּ�ó��)
                    /*
                    //�۾�����[D171][01:�԰�, 02:���, 03:�����, 04:���ġ, 05:Ȩ����][
                    byTxBuff[0] = (byte)(n_JOB_TYP_OD >> 0);
                    byTxBuff[1] = (byte)(n_JOB_TYP_OD >> 8);
                    strWriteMsg += "SC_JOB_TYP [" + n_JOB_TYP_OD + "] ";

                    //��ũ#1�۾���ȣ[D172]
                    byTxBuff[2] = (byte)(n_LUGG_NO_FK1_OD >> 0);
                    byTxBuff[3] = (byte)(n_LUGG_NO_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_LUG_NO [" + n_LUGG_NO_FK1_OD + "] ";

                    //��ũ#1����� ��(Bank)[D173]
                    byTxBuff[4] = (byte)(n_START_BANK_FK1_OD >> 0);
                    byTxBuff[5] = (byte)(n_START_BANK_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_FROM_BANK [" + n_START_BANK_FK1_OD + "] ";

                    //��ũ#1����� ��(Bay)[D174]
                    byTxBuff[6] = (byte)(n_START_BAY_FK1_OD >> 0);
                    byTxBuff[7] = (byte)(n_START_BAY_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_FROM_BAY [" + n_START_BAY_FK1_OD + "] ";

                    //��ũ#1����� ��(Level)[D175]
                    byTxBuff[8] = (byte)(n_START_LEVEL_FK1_OD >> 0);
                    byTxBuff[9] = (byte)(n_START_LEVEL_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_FROM_LEV [" + n_START_LEVEL_FK1_OD + "] ";

                    //��ũ#1����� �۾���[D176]
                    byTxBuff[10] = (byte)(n_START_HSPOS_FK1_OD >> 0);
                    byTxBuff[11] = (byte)(n_START_HSPOS_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_FROM_SITE [" + n_START_HSPOS_FK1_OD + "] ";

                    //��ũ#1������ ��(Bank)[D177]
                    byTxBuff[12] = (byte)(n_DEST_BANK_FK1_OD >> 0);
                    byTxBuff[13] = (byte)(n_DEST_BANK_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_TO_BANK [" + n_DEST_BANK_FK1_OD + "] ";

                    //��ũ#1������ ��(Bay)[D178]
                    byTxBuff[14] = (byte)(n_DEST_BAY_FK1_OD >> 0);
                    byTxBuff[15] = (byte)(n_DEST_BAY_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_TO_BAY [" + n_DEST_BAY_FK1_OD + "] ";

                    //��ũ#1������ ��(Level)[D179]
                    byTxBuff[16] = (byte)(n_DEST_LEVEL_FK1_OD >> 0);
                    byTxBuff[17] = (byte)(n_DEST_LEVEL_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_TO_LEV [" + n_DEST_LEVEL_FK1_OD + "] ";

                    //��ũ#1������ �۾���[D180]
                    byTxBuff[18] = (byte)(n_DEST_HSPOS_FK1_OD >> 0);
                    byTxBuff[19] = (byte)(n_DEST_HSPOS_FK1_OD >> 8);
                    strWriteMsg += "SC_FORK1_TO_SITE [" + n_DEST_HSPOS_FK1_OD + "] ";

                    //��ũ��� ����[D181] [0:��ũ#1�� ���, 1:��ũ#1~��ũ#2 ���û��, 2:��ũ#2�� ��� ]
                    byTxBuff[20] = (byte)(n_USE_FK_OD >> 0);
                    byTxBuff[21] = (byte)(n_USE_FK_OD >> 8);
                    strWriteMsg += "FORK��뱸�� [1] ";

                    //��ũ#2�۾���ȣ[D182]
                    byTxBuff[22] = (byte)(n_LUGG_NO_FK2_OD >> 0);
                    byTxBuff[23] = (byte)(n_LUGG_NO_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_LUG_NO [" + n_LUGG_NO_FK2_OD + "] ";

                    //��ũ#2����� ��(Bank)[D183]
                    byTxBuff[24] = (byte)(n_START_BANK_FK2_OD >> 0);
                    byTxBuff[25] = (byte)(n_START_BANK_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_FROM_BANK [" + n_START_BANK_FK2_OD + "] ";

                    //��ũ#2����� ��(Bay)[D184]
                    byTxBuff[26] = (byte)(n_START_BAY_FK2_OD >> 0);
                    byTxBuff[27] = (byte)(n_START_BAY_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_FROM_BAY [" + n_START_BAY_FK2_OD + "] ";

                    //��ũ#2����� ��(Level)[D185]
                    byTxBuff[28] = (byte)(n_START_LEVEL_FK2_OD >> 0);
                    byTxBuff[29] = (byte)(n_START_LEVEL_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_FROM_LEV [" + n_START_LEVEL_FK2_OD + "] ";

                    //��ũ#2����� �۾���[D186]
                    byTxBuff[30] = (byte)(n_START_HSPOS_FK2_OD >> 0);
                    byTxBuff[31] = (byte)(n_START_HSPOS_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_FROM_SITE [" + n_START_HSPOS_FK2_OD + "] ";

                    //��ũ#2������ ��(Bank)[D187]
                    byTxBuff[32] = (byte)(n_DEST_BANK_FK2_OD >> 0);
                    byTxBuff[33] = (byte)(n_DEST_BANK_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_TO_BANK [" + n_DEST_BANK_FK2_OD + "] ";

                    //��ũ#2������ ��(Bay)[D188]
                    byTxBuff[34] = (byte)(n_DEST_BAY_FK2_OD >> 0);
                    byTxBuff[35] = (byte)(n_DEST_BAY_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_TO_BAY [" + n_DEST_BAY_FK2_OD + "] ";

                    //��ũ#2������ ��(Level)[D189]
                    byTxBuff[36] = (byte)(n_DEST_LEVEL_FK2_OD >> 0);
                    byTxBuff[37] = (byte)(n_DEST_LEVEL_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_TO_LEV [" + n_DEST_LEVEL_FK2_OD + "] ";

                    //��ũ#2������ �۾���[D190]
                    byTxBuff[38] = (byte)(n_DEST_HSPOS_FK2_OD >> 0);
                    byTxBuff[39] = (byte)(n_DEST_HSPOS_FK2_OD >> 8);
                    strWriteMsg += "SC_FORK2_TO_SITE [" + n_DEST_HSPOS_FK2_OD + "] ";

                    //�۾�����ƮWrite Flag[D191][1�� ���� ���ݿ��� 0���� ����] [D191]
                    byTxBuff[40] = (byte)(1 >> 0);
                    byTxBuff[41] = (byte)(1 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [1] ";

                    //[D192]
                    byTxBuff[42] = (byte)(n_SC_PLT_JOB_TYP_OD >> 0);
                    byTxBuff[43] = (byte)(n_SC_PLT_JOB_TYP_OD >> 8);
                    strWriteMsg += "SC_PLT_JOB_TYP_OD [" + n_SC_PLT_JOB_TYP_OD + "] ";

                    //[D193]
                    byTxBuff[44] = (byte)(0 >> 0);
                    byTxBuff[45] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D194]
                    byTxBuff[46] = (byte)(0 >> 0);
                    byTxBuff[47] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D195]
                    byTxBuff[48] = (byte)(0 >> 0);
                    byTxBuff[49] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D196]
                    byTxBuff[50] = (byte)(0 >> 0);
                    byTxBuff[51] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D197]
                    byTxBuff[52] = (byte)(0 >> 0);
                    byTxBuff[53] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D198]
                    byTxBuff[54] = (byte)(0 >> 0);
                    byTxBuff[55] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";

                    //[D199]
                    byTxBuff[56] = (byte)(0 >> 0);
                    byTxBuff[57] = (byte)(0 >> 8);
                    strWriteMsg += "�۾�DATA WRITE [0] ";
                    //*/
                    #endregion

                    //�� ���� �� �޼��� ǥ��.
                    MakeMsg_Imp(strTitle + strWriteMsg, m_nThNo);

                    if (m_msQPlc.ReadReqeust(str_JOB_TYP, ref byTxBuff) == false)
                    {
                        //���� ��Ż��� ������Ʈ(N)
                        Communication("N", m_strWh_typ, m_strEqmtTyp, m_strPlc_No);

                        throw new Exception();
                    }

                    //���� �α� �����
                    //m_strLogMsg = strTitle + " SC_NO : [" + m_strScNo + "], �۾���ȣ : [" + str_LUGG_NO_FK1_OD + "], �۾����� : [" + Convert.ToString("" + n_JOB_TYP_OD) + "], SC ���� ����";
                    m_strLogMsg = strTitle + " SC ���� ���� [���� : " + strWriteMsg + "]";
                    if (!InsertWcsLogPgr(m_strScNo, m_strLogMsg))
                    {
                        return false;
                    }

                    //OD_RQ_YN = 'N' ������Ʈ
                    if (!UpdateMURATA_SC_OD_RQ_YN(str_LUGG_NO))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                string msg = strTitle + " " + ex.Message;
                SetErrorMsg("Comm" + m_nThNo + ex.Message);
                MakeMsg_Error(ex.Message, m_nThNo);
                InsertWcsLogPgr(m_strScNo, msg);
                return false;
            }
        }
        #endregion

        #region [UpdateMURATA_SC_DATA] :: SC_DATA_MURATA ������Ʈ �Լ�
        public bool UpdateMURATA_SC_DATA(   string strRESPONSE_CODE_RD
                                          , string strERROR_CODE_RD
                                          , string strCRANE_STATUS_RD
                                          , string strCRANE_ONLINE_RD
                                          , string strCRANE_REQUEST_RD
                                          , string strIS_ERROR_RD
                                          , string strRECOVERABLE_ERROR_RD
                                          , string strCRANE_AT_HP_RD
                                          , string strWARMING_UP_RD = "0" )
        {
            string strTitle = "[UpdateMURATA_SC_DATA] ";

            try
            {
                m_msQPlc._pBdb.BeginTrans();

                strSql = "";
                strSql += CRLF + " UPDATE SC_DATA_MURATA                                    ";
                strSql += CRLF + "    SET   RESPONSE_CODE_RD    = :RESPONSE_CODE_RD         ";
                strSql += CRLF + "        , ERROR_CODE_RD       = :ERROR_CODE_RD            ";
                strSql += CRLF + "        , CRANE_STATUS_RD     = :CRANE_STATUS_RD          ";
                strSql += CRLF + "        , CRANE_ONLINE_RD     = :CRANE_ONLINE_RD          ";
                strSql += CRLF + "        , CRANE_REQUEST_RD    = :CRANE_REQUEST_RD         ";
                strSql += CRLF + "        , IS_ERROR_RD         = :IS_ERROR_RD              ";
                strSql += CRLF + "        , RECOVERABLE_ERROR_RD= :RECOVERABLE_ERROR_RD     ";
                strSql += CRLF + "        , CRANE_AT_HP_RD      = :CRANE_AT_HP_RD           ";
                strSql += CRLF + "        , WARMING_UP_RD       = :WARMING_UP_RD            ";
                strSql += CRLF + "        , READ_UPD_DT         = " + DbLang.SYSDATE + "    ";
                strSql += CRLF + "        , OD_RQ_FLAG          = 'N'                       ";
                if (m_blHostSendYN == true) 
                {
                    strSql += CRLF + "        ,HOST_SEND_YN             = 'N'               ";
                }
                if (m_blHostErrSendYN == true)
                {
                    strSql += CRLF + "        ,HOST_ERR_SEND_YN         = 'N'               ";
                }
                strSql += CRLF + " WHERE WH_TYP = :WH_TYP                                   ";
                strSql += CRLF + "   AND PLC_NO = :PLC_NO                                   ";
                strSql += CRLF + "   AND SC_NO  = :SC_NO                                    ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("RESPONSE_CODE_RD", DbLang.VARCHAR, 255).Value = strRESPONSE_CODE_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("ERROR_CODE_RD", DbLang.VARCHAR, 255).Value = strERROR_CODE_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("CRANE_STATUS_RD", DbLang.VARCHAR, 255).Value = strCRANE_STATUS_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("CRANE_ONLINE_RD", DbLang.VARCHAR, 255).Value = strCRANE_ONLINE_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("CRANE_REQUEST_RD", DbLang.VARCHAR, 255).Value = strCRANE_REQUEST_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("IS_ERROR_RD", DbLang.VARCHAR, 255).Value = strIS_ERROR_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("RECOVERABLE_ERROR_RD", DbLang.VARCHAR, 255).Value = strRECOVERABLE_ERROR_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("CRANE_AT_HP_RD", DbLang.VARCHAR, 255).Value = strCRANE_AT_HP_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("WARMING_UP_RD", DbLang.VARCHAR, 255).Value = strWARMING_UP_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;

                ReqCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);

                if (ReqCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� ����(SC_DATA)., MSG [" + m_msQPlc._pBdb.ErrMsg + "]");
                    MakeMsg_Error(strTitle + "SC���� ���� �� ����(SC_DATA_MURATA)., MSG [" + m_msQPlc._pBdb.ErrMsg + "]", m_nThNo);
                    return false;
                }

                if (ReqCnt == 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� DATA�� �����ϴ�., PLC_NO [" + m_strPlc_No + "] " + "SC_NO [" + m_strScNo + "]");
                    MakeMsg_Error(strTitle + "SC���� ���� �� DATA�� �����ϴ�., SC_NO [" + m_strScNo.ToString() + "]", m_nThNo);
                    return false;
                }

                m_msQPlc._pBdb.Commit();
                return true;
            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� ����(SC_DATA_MURATA)., EXCEPTION MSG [" + ex.ToString() + "]");
                MakeMsg_Error(strTitle + "SC���� ���� �� ����(SC_DATA_MURATA)., EXCEPTION MSG [" + ex.ToString() + "]", m_nThNo);
                return false;
            }
        }
        #endregion

        #region [UpdateMURATA_SC_JOB_DATA] :: SC_DATA_MURATA ������Ʈ �Լ�
        public bool UpdateMURATA_SC_JOB_DATA(   string strPROD_CHECK_RD
                                              , string strORDER_CHECK_RD
                                              , string strJOB_TYPE
                                              , string strLUGG_NUM
                                              , string strINT_LUGG_NUM
                                              , string strSTART_BANK
                                              , string strSTART_BAY
                                              , string strSTART_LEVEL
                                              , string strSTART_HSPOS
                                              , string strDEST_BANK
                                              , string strDEST_BAY
                                              , string strDEST_LEVEL
                                              , string strDEST_HSPOS
                                              , string str = "0")
        {
            string strTitle = "[UpdateMURATA_SC_JOB_DATA] ";

            try
            {
                m_msQPlc._pBdb.BeginTrans();

                strSql = "";
                strSql += CRLF + " UPDATE SC_DATA_MURATA                                    ";
                strSql += CRLF + "    SET   PROD_CHECK_RD       = :PROD_CHECK_RD            ";
                strSql += CRLF + "        , ORDER_CHECK_RD      = :ORDER_CHECK_RD           ";
                strSql += CRLF + "        , JOB_TYPE            = :JOB_TYPE                 ";
                strSql += CRLF + "        , LUGG_NUM            = :LUGG_NUM                 ";
                strSql += CRLF + "        , INT_LUGG_NUM        = :INT_LUGG_NUM             ";
                strSql += CRLF + "        , START_BANK          = :START_BANK               ";
                strSql += CRLF + "        , START_BAY           = :START_BAY                ";
                strSql += CRLF + "        , START_LEVEL         = :START_LEVEL              ";
                strSql += CRLF + "        , START_HSPOS         = :START_HSPOS              ";
                strSql += CRLF + "        , DEST_BANK           = :DEST_BANK                ";
                strSql += CRLF + "        , DEST_BAY            = :DEST_BAY                 ";
                strSql += CRLF + "        , DEST_LEVEL          = :DEST_LEVEL               ";
                strSql += CRLF + "        , DEST_HSPOS          = :DEST_HSPOS               ";
                strSql += CRLF + "        , READ_UPD_DT         = " + DbLang.SYSDATE + "    ";
                strSql += CRLF + "        , OD_RQ_FLAG          = 'N'                       ";
                if (m_blHostSendYN == true)
                {
                    strSql += CRLF + "        ,HOST_SEND_YN             = 'N'               ";
                }
                if (m_blHostErrSendYN == true)
                {
                    strSql += CRLF + "        ,HOST_ERR_SEND_YN         = 'N'               ";
                }
                strSql += CRLF + " WHERE WH_TYP = :WH_TYP                                   ";
                strSql += CRLF + "   AND PLC_NO = :PLC_NO                                   ";
                strSql += CRLF + "   AND SC_NO  = :SC_NO                                    ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("PROD_CHECK_RD", DbLang.VARCHAR, 255).Value = strPROD_CHECK_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("ORDER_CHECK_RD", DbLang.VARCHAR, 255).Value = strORDER_CHECK_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("JOB_TYPE", DbLang.VARCHAR, 255).Value = strJOB_TYPE;
                m_msQPlc._pBdb.mComMain.Parameters.Add("LUGG_NUM", DbLang.VARCHAR, 255).Value = strLUGG_NUM;
                m_msQPlc._pBdb.mComMain.Parameters.Add("INT_LUGG_NUM", DbLang.VARCHAR, 255).Value = strINT_LUGG_NUM;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_BANK", DbLang.VARCHAR, 255).Value = strSTART_BANK;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_BAY", DbLang.VARCHAR, 255).Value = strSTART_BAY;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_LEVEL", DbLang.VARCHAR, 255).Value = strSTART_LEVEL;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_HSPOS", DbLang.VARCHAR, 255).Value = strSTART_HSPOS;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_BANK", DbLang.VARCHAR, 255).Value = strDEST_BANK;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_BAY", DbLang.VARCHAR, 255).Value = strDEST_BAY;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_LEVEL", DbLang.VARCHAR, 255).Value = strDEST_LEVEL;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_HSPOS", DbLang.VARCHAR, 255).Value = strDEST_HSPOS;
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;

                ReqCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);

                if (ReqCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� ����(SC_DATA)., MSG [" + m_msQPlc._pBdb.ErrMsg + "]");
                    MakeMsg_Error(strTitle + "SC���� ���� �� ����(SC_DATA_MURATA)., MSG [" + m_msQPlc._pBdb.ErrMsg + "]", m_nThNo);
                    return false;
                }

                if (ReqCnt == 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� DATA�� �����ϴ�., PLC_NO [" + m_strPlc_No + "] " + "SC_NO [" + m_strScNo + "]");
                    MakeMsg_Error(strTitle + "SC���� ���� �� DATA�� �����ϴ�., SC_NO [" + m_strScNo.ToString() + "]", m_nThNo);
                    return false;
                }

                m_msQPlc._pBdb.Commit();
                return true;
            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� ����(SC_DATA_MURATA)., EXCEPTION MSG [" + ex.ToString() + "]");
                MakeMsg_Error(strTitle + "SC���� ���� �� ����(SC_DATA_MURATA)., EXCEPTION MSG [" + ex.ToString() + "]", m_nThNo);
                return false;
            }
        }
        #endregion

        #region [UpdateSC_DATA] :: SC_DATA ������Ʈ �Լ�
        public bool UpdateSC_DATA(string  strONLINE_MODE_RD       
                                  ,string strAUTO_MODE_RD      
                                  ,string strSENSOR_FK_RD      
                                  ,string strUCSTATUS_RD       
                                  ,string strPOS_H_RD          
                                  ,string strPOS_V_RD          
                                  ,string strFORKPOS_FK1_RD    
                                  ,string strFORKPOS_FK2_RD    
                                  ,string strERR_CODE_RD       
                                  ,string strERR_STA_FK1_RD    
                                  ,string strERR_STA_FK2_RD    
                                  ,string strACTIVE_MODE_RD    
                                  ,string strCOMPLETE_RD       
                                  ,string strJOB_TYP_RD        
                                  ,string strLUGG_NO_FK1_RD    
                                  ,string strSTART_BANK_FK1_RD 
                                  ,string strSTART_HSPOS_FK1_RD
                                  ,string strDEST_BANK_FK1_RD  
                                  ,string strDEST_HSPOS_FK1_RD 
                                  ,string strUSE_FK_RD         
                                  ,string strLUGG_NO_FK2_RD    
                                  ,string strSTART_BANK_FK2_RD 
                                  ,string strSTART_HSPOS_FK2_RD
                                  ,string strDEST_BANK_FK2_RD  
                                  ,string strDEST_HSPOS_FK2_RD 
                                  ,string strSTART_BAY_FK1_RD  
                                  ,string strSTART_LEVEL_FK1_RD
                                  ,string strSTART_BAY_FK2_RD  
                                  ,string strSTART_LEVEL_FK2_RD
                                  ,string strDEST_BAY_FK1_RD   
                                  ,string strDEST_LEVEL_FK1_RD 
                                  ,string strDEST_BAY_FK2_RD   
                                  ,string strDEST_LEVEL_FK2_RD
                                  , string strSC_PLT_JOB_TYP_RD)
		{
            string strTitle = "[UpdateSC_DATA] ";

            try
            {
                m_msQPlc._pBdb.BeginTrans();

                strSql = "";
                strSql += CRLF + " UPDATE SC_DATA                                           ";
                strSql += CRLF + "    SET ONLINE_MODE_RD           = :ONLINE_MODE_RD        ";
                strSql += CRLF + "        ,AUTO_MODE_RD            = :AUTO_MODE_RD          ";
                strSql += CRLF + "        ,SENSOR_FK_RD            = :SENSOR_FK_RD          ";
                strSql += CRLF + "        ,UCSTATUS_RD             = :UCSTATUS_RD           ";
                strSql += CRLF + "        ,POS_H_RD                = :POS_H_RD              ";
                strSql += CRLF + "        ,POS_V_RD                = :POS_V_RD              ";
                strSql += CRLF + "        ,FORKPOS_FK1_RD          = :FORKPOS_FK1_RD        ";
                strSql += CRLF + "        ,FORKPOS_FK2_RD          = :FORKPOS_FK2_RD        ";
                strSql += CRLF + "        ,ERR_CODE_RD             = :ERR_CODE_RD           ";
                strSql += CRLF + "        ,ERR_STA_FK1_RD          = :ERR_STA_FK1_RD        ";
                strSql += CRLF + "        ,ERR_STA_FK2_RD          = :ERR_STA_FK2_RD        ";
                strSql += CRLF + "        ,ACTIVE_MODE_RD          = :ACTIVE_MODE_RD        ";
                strSql += CRLF + "        ,COMPLETE_RD             = :COMPLETE_RD           ";
                strSql += CRLF + "        ,JOB_TYP_RD              = :JOB_TYP_RD            ";
                strSql += CRLF + "        ,LUGG_NO_FK1_RD          = :LUGG_NO_FK1_RD        ";
                strSql += CRLF + "        ,START_BANK_FK1_RD       = :START_BANK_FK1_RD     ";
                strSql += CRLF + "        ,START_HSPOS_FK1_RD      = :START_HSPOS_FK1_RD    ";
                strSql += CRLF + "        ,DEST_BANK_FK1_RD        = :DEST_BANK_FK1_RD      ";
                strSql += CRLF + "        ,DEST_HSPOS_FK1_RD       = :DEST_HSPOS_FK1_RD     ";
                strSql += CRLF + "        ,USE_FK_RD               = :USE_FK_RD             ";
                strSql += CRLF + "        ,LUGG_NO_FK2_RD          = :LUGG_NO_FK2_RD        ";
                strSql += CRLF + "        ,START_BANK_FK2_RD       = :START_BANK_FK2_RD     ";
                strSql += CRLF + "        ,START_HSPOS_FK2_RD      = :START_HSPOS_FK2_RD    ";
                strSql += CRLF + "        ,DEST_BANK_FK2_RD        = :DEST_BANK_FK2_RD      ";
                strSql += CRLF + "        ,DEST_HSPOS_FK2_RD       = :DEST_HSPOS_FK2_RD     ";
                strSql += CRLF + "        ,START_BAY_FK1_RD        = :START_BAY_FK1_RD      ";
                strSql += CRLF + "        ,START_LEVEL_FK1_RD      = :START_LEVEL_FK1_RD    ";
                strSql += CRLF + "        ,START_BAY_FK2_RD        = :START_BAY_FK2_RD      ";
                strSql += CRLF + "        ,START_LEVEL_FK2_RD      = :START_LEVEL_FK2_RD    ";
                strSql += CRLF + "        ,DEST_BAY_FK1_RD         = :DEST_BAY_FK1_RD       ";
                strSql += CRLF + "        ,DEST_LEVEL_FK1_RD       = :DEST_LEVEL_FK1_RD     ";
                strSql += CRLF + "        ,DEST_BAY_FK2_RD         = :DEST_BAY_FK2_RD       ";
                strSql += CRLF + "        ,DEST_LEVEL_FK2_RD       = :DEST_LEVEL_FK2_RD     ";
                strSql += CRLF + "        ,SC_PLT_JOB_TYP_RD       = :SC_PLT_JOB_TYP_RD     ";
                strSql += CRLF + "        ,READ_UPD_DT             = " + DbLang.SYSDATE + " ";
                strSql += CRLF + "        ,OD_RQ_FLAG              = 'N'                    ";
                if (m_blHostSendYN == true)
                {
                    strSql += CRLF + "        ,HOST_SEND_YN             = 'N'               ";
                }
                if (m_blHostErrSendYN == true)
                {
                    strSql += CRLF + "        ,HOST_ERR_SEND_YN         = 'N'               ";
                }
                strSql += CRLF + " WHERE WH_TYP = :WH_TYP                                   ";
                strSql += CRLF + "   AND PLC_NO = :PLC_NO                                   ";
                strSql += CRLF + "   AND SC_NO  = :SC_NO                                    ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("ONLINE_MODE_RD", DbLang.VARCHAR, 255).Value = strONLINE_MODE_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("AUTO_MODE_RD", DbLang.VARCHAR, 255).Value = strAUTO_MODE_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SENSOR_FK_RD", DbLang.VARCHAR, 255).Value = strSENSOR_FK_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("UCSTATUS_RD", DbLang.VARCHAR, 255).Value = strUCSTATUS_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("POS_H_RD", DbLang.VARCHAR, 255).Value = strPOS_H_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("POS_V_RD", DbLang.VARCHAR, 255).Value = strPOS_V_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("FORKPOS_FK1_RD", DbLang.VARCHAR, 255).Value = strFORKPOS_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("FORKPOS_FK2_RD", DbLang.VARCHAR, 255).Value = strFORKPOS_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("ERR_CODE_RD", DbLang.VARCHAR, 255).Value = strERR_CODE_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("ERR_STA_FK1_RD", DbLang.VARCHAR, 255).Value = strERR_STA_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("ERR_STA_FK2_RD", DbLang.VARCHAR, 255).Value = strERR_STA_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("ACTIVE_MODE_RD", DbLang.VARCHAR, 255).Value = strACTIVE_MODE_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("COMPLETE_RD", DbLang.VARCHAR, 255).Value = strCOMPLETE_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("JOB_TYP_RD", DbLang.VARCHAR, 255).Value = strJOB_TYP_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("LUGG_NO_FK1_RD", DbLang.VARCHAR, 255).Value = strLUGG_NO_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_BANK_FK1_RD", DbLang.VARCHAR, 255).Value = strSTART_BANK_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_HSPOS_FK1_RD", DbLang.VARCHAR, 255).Value = strSTART_HSPOS_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_BANK_FK1_RD", DbLang.VARCHAR, 255).Value = strDEST_BANK_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_HSPOS_FK1_RD", DbLang.VARCHAR, 255).Value = strDEST_HSPOS_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("USE_FK_RD", DbLang.VARCHAR, 255).Value = strUSE_FK_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("LUGG_NO_FK2_RD", DbLang.VARCHAR, 255).Value = strLUGG_NO_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_BANK_FK2_RD", DbLang.VARCHAR, 255).Value = strSTART_BANK_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_HSPOS_FK2_RD", DbLang.VARCHAR, 255).Value = strSTART_HSPOS_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_BANK_FK2_RD", DbLang.VARCHAR, 255).Value = strDEST_BANK_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_HSPOS_FK2_RD", DbLang.VARCHAR, 255).Value = strDEST_HSPOS_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_BAY_FK1_RD", DbLang.VARCHAR, 255).Value = strSTART_BAY_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_LEVEL_FK1_RD", DbLang.VARCHAR, 255).Value = strSTART_LEVEL_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_BAY_FK2_RD", DbLang.VARCHAR, 255).Value = strSTART_BAY_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("START_LEVEL_FK2_RD", DbLang.VARCHAR, 255).Value = strSTART_LEVEL_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_BAY_FK1_RD", DbLang.VARCHAR, 255).Value = strDEST_BAY_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_LEVEL_FK1_RD", DbLang.VARCHAR, 255).Value = strDEST_LEVEL_FK1_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_BAY_FK2_RD", DbLang.VARCHAR, 255).Value = strDEST_BAY_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("DEST_LEVEL_FK2_RD", DbLang.VARCHAR, 255).Value = strDEST_LEVEL_FK2_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_PLT_JOB_TYP_RD", DbLang.VARCHAR, 255).Value = strSC_PLT_JOB_TYP_RD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;

                ReqCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);

                if (ReqCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� ����(SC_DATA)., MSG [" + m_msQPlc._pBdb.ErrMsg + "]");
                    MakeMsg_Error(strTitle + "SC���� ���� �� ����(SC_DATA)., MSG [" + m_msQPlc._pBdb.ErrMsg + "]", m_nThNo);
                    return false;
                }

                if (ReqCnt == 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� DATA�� �����ϴ�., PLC_NO [" + m_strPlc_No + "] " + "SC_NO [" + m_strScNo + "]");
                    MakeMsg_Error(strTitle + "SC���� ���� �� DATA�� �����ϴ�., SC_NO [" + m_strScNo.ToString() + "]", m_nThNo);
                    return false;
                }

                m_msQPlc._pBdb.Commit();
                return true;
            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                SetErrorMsg("Comm" + m_nThNo + " :" + strTitle + "SC���� ���� �� ����(SC_DATA)., EXCEPTION MSG [" + ex.ToString() + "]");
                MakeMsg_Error(strTitle + "SC���� ���� �� ����(SC_DATA)., EXCEPTION MSG [" + ex.ToString() + "]", m_nThNo);
                return false;
            }
		}
        #endregion

        #region [InsertWcsLogPgr] :: WCS_LOG_PGR�� LOG �����
        public bool InsertWcsLogPgr(string strTRACK_NO, string strLOG_MSG)
        {
            try
            {
                m_msQPlc._pBdb.BeginTrans();

                strSql = "";
                strSql += CRLF + "INSERT INTO WCS_LOG_PGR (WH_TYP                ";
                strSql += CRLF + "						  ,INS_DT                ";
                strSql += CRLF + "						  ,LOG_SEQ               ";
                strSql += CRLF + "						  ,LUGG_NO               ";
                strSql += CRLF + "						  ,BCR_BOTTOM            ";
                strSql += CRLF + "						  ,BCR_TOP               ";
                strSql += CRLF + "						  ,PGR_NM                ";
                strSql += CRLF + "						  ,LOG_KOR               ";
                strSql += CRLF + "						  ,TRACK_FROM            ";
                strSql += CRLF + "						  ,TRACK_TO              ";
                strSql += CRLF + "						  ,JOB_STA               ";
                strSql += CRLF + "						  ,RQ_INS_ID             ";
                strSql += CRLF + "						  ,RQ_INS_DT             ";
                strSql += CRLF + "						  ,EQP_TYP )             ";
                strSql += CRLF + "				VALUES    (:WH_TYP               ";
                strSql += CRLF + "						  ," + DbLang.SYSDATE + "";
                strSql += CRLF + "						  ,NEXTVAL('LOG_SEQ')    ";
                strSql += CRLF + "						  ,NULL                  ";
                strSql += CRLF + "						  ,NULL                  ";
                strSql += CRLF + "						  ,NULL                  ";
                strSql += CRLF + "						  ,:PGR_NM               ";
                strSql += CRLF + "						  ,:LOG_KOR              ";
                strSql += CRLF + "						  ,NULL                  ";
                strSql += CRLF + "						  ,NULL                  ";
                strSql += CRLF + "						  ,:JOB_STA              ";
                strSql += CRLF + "						  ,:RQ_INS_ID            ";
                strSql += CRLF + "						  ," + DbLang.SYSDATE + "";
                strSql += CRLF + "						  ,:EQP_TYP )            ";


                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();

                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PGR_NM", DbLang.VARCHAR, 255).Value = m_strLogFileNm;
                m_msQPlc._pBdb.mComMain.Parameters.Add("LOG_KOR", DbLang.VARCHAR, 255).Value = strLOG_MSG;
                m_msQPlc._pBdb.mComMain.Parameters.Add("JOB_STA", DbLang.VARCHAR, 255).Value = "999";
                m_msQPlc._pBdb.mComMain.Parameters.Add("RQ_INS_ID", DbLang.VARCHAR, 255).Value = strTRACK_NO;
                m_msQPlc._pBdb.mComMain.Parameters.Add("EQP_TYP", DbLang.VARCHAR, 255).Value = m_strEqmtTyp;
                nSelCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);

                if (nSelCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    SetErrorMsg("Comm" + m_nThNo + " :[InsertWcsLogPgr] �������� �� ���°� ������ ERROR., PLC_NO [" + m_strPlc_No + "] TRACK_NO [" + strTRACK_NO + "] MSG [" + m_msQPlc._pBdb.ErrMsg + "]");
                    MakeMsg_Error("[InsertWcsLogPgr] �������� �� ���°� ������ ERROR., PLC_NO [" + m_strPlc_No + "] TRACK_NO [" + strTRACK_NO + "] MSG [" + m_msQPlc._pBdb.ErrMsg + "]", m_nThNo);
                    return false;
                }

                if (nSelCnt == 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    SetErrorMsg("Comm" + m_nThNo + " :[InsertWcsLogPgr]�������� �� ���°� ������ DATA�� �����ϴ�., PLC_NO [" + m_strPlc_No + "] TRACK_NO [" + strTRACK_NO + "]");
                    MakeMsg_Error("[InsertWcsLogPgr] �������� �� ���°� ������ DATA�� �����ϴ�.,PLC_NO [" + m_strPlc_No + "]  TRACK_NO [" + strTRACK_NO + "]", m_nThNo);
                    return false;

                }

                m_msQPlc._pBdb.Commit();
                return true;

            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                SetErrorMsg("Comm" + m_nThNo + " :[InsertWcsLogPgr] �������� �� ���°� ������ ERROR., PLC_NO [" + m_strPlc_No + "] TRACK_NO  [" + strTRACK_NO + "] MSG [" + ex.ToString() + "]");
                MakeMsg_Error("[InsertWcsLogPgr] �������� �� ���°� ������ ERROR., PLC_NO [" + strTRACK_NO + "] MSG [" + ex.ToString() + "]", m_nThNo);
                return false;
            }
        }
        #endregion

        #region [Communication] :: EQP_MST�� CONNECT ���� ����
        public bool Communication(string CONNECTED_YN, string WH_TYP, string EQP_TYP, string PLC_NO)
        {
            string strTitle = "[Communication]";

            try
            {
                m_msQPlc._pBdb.BeginTrans();

                string strSql = "";
                string CRLF = "\r\n";
                int nSelCnt;

                MakeMsg("PLC ��� OK", m_nThNo);

                strSql = "";
                strSql += CRLF + "UPDATE EQP_MST                                    ";
                strSql += CRLF + "   SET CONNECTED_YN      = :CONNECTED_YN          ";
                strSql += CRLF + "      ,UPD_DT            = " + DbLang.SYSDATE + " ";
                strSql += CRLF + "      ,PLC_PORT          = :PLC_PORT              ";
                strSql += CRLF + "WHERE  WH_TYP            = :WH_TYP                ";
                strSql += CRLF + "AND    EQP_TYP           = :EQP_TYP               ";
                strSql += CRLF + "AND    PLC_NO            = :PLC_NO                ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("CONNECTED_YN", DbLang.VARCHAR).Value = CONNECTED_YN;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_PORT", DbLang.VARCHAR, 255).Value = Convert.ToString("" + m_nCurPort);
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = WH_TYP;
                m_msQPlc._pBdb.mComMain.Parameters.Add("EQP_TYP", DbLang.VARCHAR, 255).Value = EQP_TYP;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = PLC_NO;
                nSelCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);
                if (nSelCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    MakeMsg_Error(strTitle + "PLC���� ������ ERROR. ErrorMsg [" + m_msQPlc._pBdb.ErrMsg + "] WH_TYP [" + WH_TYP + "] EQP_TYP [" + EQP_TYP + "]  PLC_NO [" + PLC_NO + "]", m_nThNo);
                    return false;
                }

                if (nSelCnt == 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    MakeMsg_Error(strTitle + "PLC���� ������ Data�� �����ϴ�.WH_TYP [" + WH_TYP + "] EQP_TYP [" + EQP_TYP + "] PLC_NO [" + PLC_NO + "] CONNECTED_YN [" + CONNECTED_YN + "]", m_nThNo);
                    return false;
                }

                m_msQPlc._pBdb.Commit();
                return true;
            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                MakeMsg_Error(strTitle + "Exception Error" + ex.Message, m_nThNo);
                return false;
            }
        }
        #endregion

        #region [InsertEQMT_ERR_LOG] :: SC �������¸� �̷¿� �����
        public bool InsertEQMT_ERR_LOG(string pWH_TYP,
                                       string pEQP_TYP,
                                       string pEQP_NO,
                                       string pEQP_ERR_CD,
                                       string pLUGG_NO)
        {
            try
            {
                m_msQPlc._pBdb.BeginTrans();

                strSql = "";
                strSql += CRLF + "INSERT INTO EQP_ERR_HIS (WH_TYP                 ";
                strSql += CRLF + "                       , EQP_TYP                ";
                strSql += CRLF + "                       , EQP_NO                 ";
                strSql += CRLF + "                       , ERROR_DT               ";
                strSql += CRLF + "                       , EQP_ERR_CD             ";
                strSql += CRLF + "                       , BCR_BOTTOM             ";
                strSql += CRLF + "                       , BCR_TOP                ";
                strSql += CRLF + "                       , LUGG_NO)               ";
                strSql += CRLF + "                VALUES  (:WH_TYP                ";
                strSql += CRLF + "                       , :EQP_TYP               ";
                strSql += CRLF + "                       , :EQP_NO                ";
                strSql += CRLF + "                       , " + DbLang.SYSDATE + " ";
                strSql += CRLF + "                       , :EQP_ERR_CD            ";
                strSql += CRLF + "                       , null                   ";
                strSql += CRLF + "                       , null                   ";
                strSql += CRLF + "                       , :LUGG_NO)              ";



                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = pWH_TYP;
                m_msQPlc._pBdb.mComMain.Parameters.Add("EQP_TYP", DbLang.VARCHAR, 255).Value = pEQP_TYP;
                m_msQPlc._pBdb.mComMain.Parameters.Add("EQP_NO", DbLang.VARCHAR, 255).Value = pEQP_NO;
                m_msQPlc._pBdb.mComMain.Parameters.Add("EQP_ERR_CD", DbLang.VARCHAR, 255).Value = pEQP_ERR_CD;
                m_msQPlc._pBdb.mComMain.Parameters.Add("LUGG_NO", DbLang.VARCHAR, 255).Value = pLUGG_NO;
                nSelCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);

                if (nSelCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    MakeMsg_Error("[InsertEQMT_ERR_LOG]:: Error:PLC���� ���� �α� ���� ", m_nThNo);
                    return false;
                }

                m_msQPlc._pBdb.Commit();
                return true;
            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                MakeMsg_Error("[InsertEQMT_ERR_LOG]:: Error:PLC���� ���� �α� Exception ���� ���� ", m_nThNo);
                return false;
            }
        }
        #endregion

        #region [UpdateSC_CMD_RQ_YN] :: SC_DATA�� CMD_RQ_YN = 'N'���� ������Ʈ
        public bool UpdateSC_CMD_RQ_YN(string CMD_RQ_ID)
        {
            try
            {
                m_msQPlc._pBdb.BeginTrans();

                strSql = "";
                strSql += CRLF + "UPDATE SC_DATA                               ";
                strSql += CRLF + "   SET CMD_RQ_YN = 'N'                       ";
                strSql += CRLF + "      ,WRITE_UPD_DT = " + DbLang.SYSDATE + " ";
                if (CMD_RQ_ID == "DELFK1")
                {
                    strSql += CRLF + "       ,ITN_LUGG_FK1 = '0'    ";
                }
                if (CMD_RQ_ID == "DELFK2")
                {
                    strSql += CRLF + "       ,ITN_LUGG_FK2 = '0'    ";
                }
                if (CMD_RQ_ID == "DELFK12")
                {
                    strSql += CRLF + "       ,ITN_LUGG_FK1 = '0'    ";
                    strSql += CRLF + "       ,ITN_LUGG_FK2 = '0'    ";
                }
                strSql += CRLF + " WHERE WH_TYP = :WH_TYP           ";
                strSql += CRLF + "   AND PLC_NO = :PLC_NO           ";
                strSql += CRLF + "   AND SC_NO = :SC_NO             ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;
                nSelCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);

                if (nSelCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    MakeMsg_Error("[UpdateSC_CMD_RQ_YN]:: Error:CMD_RQ_YN ���� ���� ", m_nThNo);
                    return false;
                }

                m_msQPlc._pBdb.Commit();
                return true;
            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                MakeMsg_Error("[UpdateSC_CMD_RQ_YN]:: Error:CMD_RQ_YN ���� ���� ", m_nThNo);
                return false;
            }
        }
        #endregion

        #region [UpdateSC_OD_RQ_YN] :: SC_DATA�� OD_RQ_YN = 'N'���� ������Ʈ
        public bool UpdateSC_OD_RQ_YN(string pITN_LUGG_FK1, string pITN_LUGG_FK2)
        {
            try
            {
                m_msQPlc._pBdb.BeginTrans();

                strSql = "";
                strSql += CRLF + "UPDATE SC_DATA                                ";
                strSql += CRLF + "   SET OD_RQ_YN      = 'N'                    ";
                strSql += CRLF + "       ,ITN_LUGG_FK1 = :ITN_LUGG_FK1          ";
                strSql += CRLF + "       ,ITN_LUGG_FK2 = :ITN_LUGG_FK2          ";
                strSql += CRLF + "       ,WRITE_UPD_DT = " + DbLang.SYSDATE + " ";
                strSql += CRLF + "       ,OD_RQ_FLAG   = 'Y'                    ";
                strSql += CRLF + " WHERE WH_TYP = :WH_TYP                       ";
                strSql += CRLF + "   AND PLC_NO = :PLC_NO                       ";
                strSql += CRLF + "   AND SC_NO = :SC_NO                         ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("ITN_LUGG_FK1", DbLang.VARCHAR, 255).Value = pITN_LUGG_FK1;
                m_msQPlc._pBdb.mComMain.Parameters.Add("ITN_LUGG_FK2", DbLang.VARCHAR, 255).Value = pITN_LUGG_FK2;
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;
                nSelCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);

                if (nSelCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    MakeMsg_Error("[UpdateSC_OD_RQ_YN]:: Error:OD_RQ_YN ���� ���� ", m_nThNo);
                    return false;
                }

                m_msQPlc._pBdb.Commit();
                return true;
            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                MakeMsg_Error("[UpdateSC_OD_RQ_YN]:: Error:OD_RQ_YN ���� ���� ", m_nThNo);
                return false;
            }
        }
        #endregion

        #region [UpdateMURATA_SC_OD_RQ_YN] :: SC_DATA_MURATA�� OD_RQ_YN = 'N'���� ������Ʈ
        public bool UpdateMURATA_SC_OD_RQ_YN(string pITN_LUGG_FK1)
        {
            try
            {
                m_msQPlc._pBdb.BeginTrans();

                strSql = "";
                strSql += CRLF + "UPDATE SC_DATA_MURATA                         ";
                strSql += CRLF + "   SET OD_RQ_YN      = 'N'                    ";
                strSql += CRLF + "       ,ITN_LUGG_FK1 = :ITN_LUGG              ";
                strSql += CRLF + "       ,WRITE_UPD_DT = " + DbLang.SYSDATE + " ";
                strSql += CRLF + "       ,OD_RQ_FLAG   = 'Y'                    ";
                strSql += CRLF + " WHERE WH_TYP = :WH_TYP                       ";
                strSql += CRLF + "   AND PLC_NO = :PLC_NO                       ";
                strSql += CRLF + "   AND SC_NO = :SC_NO                         ";

                m_msQPlc._pBdb.mComMain.CommandType = CommandType.Text;
                m_msQPlc._pBdb.mComMain.Parameters.Clear();
                m_msQPlc._pBdb.mComMain.Parameters.Add("ITN_LUGG_FK1", DbLang.VARCHAR, 255).Value = pITN_LUGG_FK1;
                m_msQPlc._pBdb.mComMain.Parameters.Add("WH_TYP", DbLang.VARCHAR, 255).Value = m_strWh_typ;
                m_msQPlc._pBdb.mComMain.Parameters.Add("PLC_NO", DbLang.VARCHAR, 255).Value = m_strPlc_No;
                m_msQPlc._pBdb.mComMain.Parameters.Add("SC_NO", DbLang.VARCHAR, 255).Value = m_strScNo;
                nSelCnt = m_msQPlc._pBdb.ExcuteNonQry(strSql);

                if (nSelCnt < 0)
                {
                    m_msQPlc._pBdb.Rollback();
                    MakeMsg_Error("[UpdateMURATA_SC_OD_RQ_YN]:: Error:OD_RQ_YN ���� ���� ", m_nThNo);
                    return false;
                }

                m_msQPlc._pBdb.Commit();
                return true;
            }
            catch (Exception ex)
            {
                m_msQPlc._pBdb.Rollback();
                MakeMsg_Error("[UpdateMURATA_SC_OD_RQ_YN]:: Error:OD_RQ_YN ���� ���� ", m_nThNo);
                return false;
            }
        }
        #endregion
    }
}