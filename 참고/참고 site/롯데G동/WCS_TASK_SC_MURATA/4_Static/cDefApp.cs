using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Threading;
using Microsoft.VisualBasic;
using Npgsql;

namespace WCS_TASK_SC
{
    public class cDefApp
    {
        public static OleDbConnection GM_DB1_CN;            // @.DB���� ����[OracleConnection]
        public static NpgsqlConnection GM_DB2_CN;            // @.DB���� ����[NpgsqlConnection]

        public const string GM_ENV_INI = "./WCS_DB.INI";   // @.INI ���ϰ��

        public static string GM_CNF_USER_ID = "T-SC";

        // @@.DB ����
        public const string DB_TYPE_NONE = "0";
        public const string DB_TYPE_ORACLE = "1";
        public const string DB_TYPE_POSTGRESQL = "2";
        public const string DB_TYPE_MS_SQL = "3";
        public const string DB_TYPE_MY_SQL = "4";
        public const string DB_TYPE_SIZE = "5";

        // @@.[DB_TYPE] ��������
        public static string DB_TYPE_INI = "0"; // 0:None, 1:Oracle, 2:PostgreSql, 3:MS_SQL, 4:MY_SQL

        // @@.[DB_1] Oracle ��������
        public static string GM_DB1_PROVIDER = "";
        public static string GM_DB1_ALIAS = "";
        public static string GM_DB1_USERID = "";
        public static string GM_DB1_PASSWORD = "";

        // @@.[DB_2] PostgreSql ��������
        public static string GM_DB2_IP = "";
        public static string GM_DB2_DATABASE = "";
        public static string GM_DB2_PORT = "";
        public static string GM_DB2_USER = "";
        public static string GM_DB2_USER_PW = "";

        // @@.������� ����
        // [CNF]
        public static string GM_WH_TYP = "";
        public static string GM_USERID = "";


        // @@.������� Ÿ�Ӿƿ� ����
        public static int GM_COMM_SND_TIME_OUT = 500;
        public static int GM_COMM_RCV_TIME_OUT = 500;

        public static Queue<LogParam>[] m_LogQ = new Queue<LogParam>[200];

        // @@.Application ���� ���� ���� ���� ����
        public static bool  GM_STAT_MAIN = false;  // @.��ü �ý��� ���� ����[���� �ý����� ���� �Ǹ� ��ü ����!] 
        public static bool GM_RE_START = false;

        // @@.������ ����
        //public Thread  GM_TH_WC1;     // @.W/C1�� ����ϴ� ������[���ϼӼ�:Client]
    
        // @@.������
        public const string cSPA = ";";

        // @@.���ó���۾�����ü
        public struct stutComProc
        {
            public bool    bMakeTgmSnd;     // @.����Tgm�ۼ�
            public bool   bSndTgm;          // @.Tgm����
            public bool   bRcvTgm;          // @.Tgm����
            public int   nChkTgm;           // @.����Tgmüũ
            public bool   bDBProc;          // @.DBó��
            public string   sProcMsg;       // @.ó���޼���

            public void  init()
                {                               // @.����ü ���𺯼� �ʱ�ȭ
                this.bMakeTgmSnd = false;         // @.����Tgm�ۼ�
                this.bSndTgm = false;             // @.Tgm����
                this.bRcvTgm = false;             // @.Tgm����
                this.nChkTgm = 99;                // @.����Tgmüũ
                this.bDBProc = false;             // @.DBó��
                this.sProcMsg = "";               // @.ó���޼���
                }
        }

        // @@.DB Err ���
        public const int DB_ERR = -1;      // @.DB ����
        public const int DB_LOCK = -2 ;    // @.DB ������ DB Lock
        public const int DB_DUP = -3;      // @.DB ������ �ߺ� ����Ÿ

        // @@.enum ����
        public enum eComSts{ComNor = 0, ComErr = 1};                     // @.��Ż���
        public enum eLogMsgType{MSG_NOR = 0,MSG_IMP = 1,MSG_ERR = 2  };  // @.eLogMsgType[0:����, 1:�߿�, 2:����]
        public enum eLogWriteGbn { COMM1 = 0, COMM2 = 1, COMM3 = 2, COMM4 = 3, COMM5 = 4, COMM6 = 5, COMM7 = 6, COMM8 = 7, COMM9 = 8, COMM10 = 9 };
        
        // @@.Structure ����
        public struct stutLogMsgInfo 
        {
            public string  Time;  
            public string  ID;  
            public string  MsgTyp; 
            public string  Com;  
            public string  Msg;  
            public string  Tgm;  
            public void  init()
            {
                this.Time = "";
                this.ID = "";
                this.MsgTyp = "";
                this.Com = "";
                this.Msg = "";
                this.Tgm = "";
            }
        }

        public const string  CRLF  = ControlChars.CrLf; // @.�����[vbCrLf]
        public const byte  STX  =   0x2 ;
        public const byte ETX   =  0x3 ;
        public   char  GM_STR_STX = Convert.ToChar(2);
        public   char GM_STR_ETX = Convert.ToChar(3);

    }
}
