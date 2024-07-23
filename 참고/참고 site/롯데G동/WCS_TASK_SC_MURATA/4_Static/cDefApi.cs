using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WCS_TASK_SC
{
    class cDefApi
    {
        // @@@.INI���Ͽ��� ������ �����͸� �о��.
        [DllImport("kernel32.dll")]
        static extern int GetPrivateProfileInt(string lpApplicationName, string lpKeyName, int nDefault, string lpFileName);

        // @@@.INI���Ͽ��� ������ �����͸� �о��.
        [DllImport("kernel32.dll")]
        static extern uint GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        // @@@. INI���Ͽ� ����.
        [DllImport("kernel32.dll")]
        public static extern uint WritePrivateProfileString(string section, string key, string val, string filePath);

        // @@@.GsGetInitPorFileDB
        public static void GsGetStringInitPorFile(string strAppName,
                                                  string strKeyName,
                                                  ref string strValue,
                                                  ref string strRtnMsg)
        {
            string strTitle = "[GsGetStringInitPorFile] ";

            strValue = "";

            StringBuilder sb = new StringBuilder(1000);
            if (!System.IO.File.Exists(cDefApp.GM_ENV_INI))
            {
                strRtnMsg = strTitle + "Not Found File";
                return;
            }

            try
            {
                strRtnMsg = "";
                GetPrivateProfileString(strAppName, strKeyName, null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                strValue = sb.ToString();
                strRtnMsg = strTitle + "Sucess";
                return;
            }
            catch (Exception ex)
            {
                strRtnMsg = strTitle + ex.Message;
                return;
            }
        }

        // @@@.GsGetInitPorFileDB
        public static void GsGetIntInitPorFile(string strAppName,
                                              string strKeyName,
                                              ref int nValue,
                                              ref string strRtnMsg)
        {
            string strTitle = "[GsGetIntInitPorFile] ";

            nValue = 0;
            if (!System.IO.File.Exists(cDefApp.GM_ENV_INI))
            {
                strRtnMsg = strTitle + "Not Found File";
                return;
            }

            try
            {
                strRtnMsg = "";
                nValue = GetPrivateProfileInt(strAppName, strKeyName, nValue, cDefApp.GM_ENV_INI);
                strRtnMsg = strTitle + "Sucess";
                return;
            }
            catch (Exception ex)
            {
                strRtnMsg = strTitle + ex.Message;
                return;
            }
        }

        #region [DB_TYPE]::DB ���� ��������
        public static void GsGetInitPorFileDB_TYPE(ref string pTYPE,
                                                   ref string pRtnMsg)
        {
            StringBuilder sb = new StringBuilder(1000);

            if (!System.IO.File.Exists(cDefApp.GM_ENV_INI))
            {
                pRtnMsg = "[GsGetInitPorFileDB_TYPE]::Not Found File";
                return;
            }

            try
            {
                pRtnMsg = "";

                GetPrivateProfileString("DB_TYPE", "TYPE", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pTYPE = sb.ToString();

                pRtnMsg = "[GsGetInitPorFileDB_TYPE]::Sucess";
                return;
            }
            catch (Exception ex)
            {
                pRtnMsg = ex.Message;
            }
            pRtnMsg = "[GsGetInitPorFileDB_TYPE]Error::" + pRtnMsg;
        }
        #endregion

        #region [DB_1]::Oracle ��������
        public static void GsGetInitPorFileDB_1(ref string pProvider,
                                              ref string pAlias,
                                              ref string pUserID,
                                              ref string pPassword,
                                              ref string pRtnMsg)
        {
            StringBuilder sb = new StringBuilder(1000);

            if (!System.IO.File.Exists(cDefApp.GM_ENV_INI))
            {
                pRtnMsg = "[GsGetInitPorFileDB_1]::Not Found File";
                return;
            }

            try
            {
                pRtnMsg = "";

                GetPrivateProfileString("DB_1", "PROVIDER", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pProvider = sb.ToString();

                GetPrivateProfileString("DB_1", "ALIAS", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pAlias = sb.ToString();

                GetPrivateProfileString("DB_1", "USERID", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pUserID = sb.ToString();

                GetPrivateProfileString("DB_1", "PASSWORD", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pPassword = sb.ToString();

                pRtnMsg = "[GsGetInitPorFileDB_1]::Sucess";
                return;
            }
            catch (Exception ex)
            {
                pRtnMsg = ex.Message;
            }
            pRtnMsg = "[GsGetInitPorFileDB]Error::" + pRtnMsg;
        }
        #endregion

        #region [DB_2]::PostgreSql ��������
        public static void GsGetInitPorFileDB_2(ref string pIP,
                                              ref string pDATABASE,
                                              ref string pPORT,
                                              ref string pUSER,
                                              ref string pUSER_PW,
                                              ref string pRtnMsg)
        {
            StringBuilder sb = new StringBuilder(1000);

            if (!System.IO.File.Exists(cDefApp.GM_ENV_INI))
            {
                pRtnMsg = "[GsGetInitPorFileDB_2]::Not Found File";
                return;
            }

            try
            {
                pRtnMsg = "";

                GetPrivateProfileString("DB_2", "IP", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pIP = sb.ToString();

                GetPrivateProfileString("DB_2", "DATABASE", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pDATABASE = sb.ToString();

                GetPrivateProfileString("DB_2", "PORT", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pPORT = sb.ToString();

                GetPrivateProfileString("DB_2", "USER", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pUSER = sb.ToString();

                GetPrivateProfileString("DB_2", "USER_PW", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pUSER_PW = sb.ToString();

                pRtnMsg = "[GsGetInitPorFileDB_2]::Sucess";
                return;
            }
            catch (Exception ex)
            {
                pRtnMsg = ex.Message;
            }
            pRtnMsg = "[GsGetInitPorFileDB_2]Error::" + pRtnMsg;
        }
        #endregion

        #region [CNF]::��������
        public static void GsGetInitPorFileCNF(ref string pGRP,
                                              ref string pUSERID,
                                              ref string pRtnMsg)
        {
            StringBuilder sb = new StringBuilder(1000);

            if (!System.IO.File.Exists(cDefApp.GM_ENV_INI))
            {
                pRtnMsg = "[GsGetInitPorFileCNF]::Not Found File";
                return;
            }

            try
            {
                pRtnMsg = "";

                GetPrivateProfileString("CNF", "WH_TYP", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pGRP = sb.ToString();

                GetPrivateProfileString("CNF", "USERID", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pUSERID = sb.ToString();

                pRtnMsg = "[GsGetInitPorFileCNF]::Sucess";
                return;
            }
            catch (Exception ex)
            {
                pRtnMsg = ex.Message;
            }
            pRtnMsg = "[GsGetInitPorFileCNF]Error::" + pRtnMsg;
        }
        #endregion

        #region [PROCESS]::CNT ��������
        public static bool GsReadInitProfileProcessCnt(string pAppNm, ref int pProcessCnt, ref string pRtnMsg)
        {
            StringBuilder sb = new StringBuilder(1000);

            if (!System.IO.File.Exists(cDefApp.GM_ENV_INI))
            {
                pRtnMsg = "[GsReadInitProfileProcessCnt]::Not Found File";
                return false;
            }

            try
            {
                pRtnMsg = "";

                pProcessCnt = GetPrivateProfileInt(pAppNm, "CNT", 1, cDefApp.GM_ENV_INI);

                pRtnMsg = "[GsReadInitProfileProcessCnt]::Sucess";
                return true;
            }
            catch (Exception ex)
            {
                pRtnMsg = ex.Message;
            }
            pRtnMsg = "[GsReadInitProfileProcessCnt]Error::" + pRtnMsg;
            return false;
        }
        #endregion

        #region [COMM]::������� ��������
        public static bool GsReadInitProfileCom(string pAppNm,
                                            ref string pGrpNo,
                                            ref string pCommIP,
                                            ref string pComCurPort,
                                            ref string pComFromPort,
                                            ref string pComToPort,
                                            ref    int i,
                                            ref string pLogPath,
                                            ref string pLogFileNm,
                                            ref string pEqmt,
                                            ref string pScNo,
                                            ref string pMcNo,
                                            ref string pScGrpNo,
                                            ref int    pPortCnt,
                                            ref string pRtnMsg)
        {
            StringBuilder sb = new StringBuilder(1000);

            if (!System.IO.File.Exists(cDefApp.GM_ENV_INI))
            {
                pRtnMsg = "[GsReadInitProfileCom]::Not Found File";
                return false;
            }

            try
            {
                pRtnMsg = "";

                GetPrivateProfileString(pAppNm, "EQMT", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pEqmt = sb.ToString();

                GetPrivateProfileString(pAppNm, "IP", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pCommIP = sb.ToString();

                GetPrivateProfileString(pAppNm, "CUR_PORT", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pComCurPort = sb.ToString();

                GetPrivateProfileString(pAppNm, "FROM_PORT", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pComFromPort = sb.ToString();

                GetPrivateProfileString(pAppNm, "TO_PORT", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pComToPort = sb.ToString();

                GetPrivateProfileString(pAppNm, "PLC_NO", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pGrpNo = sb.ToString();

                GetPrivateProfileString(pAppNm, "SC_NO", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pScNo = sb.ToString();

                GetPrivateProfileString(pAppNm, "MC_NO", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pMcNo = sb.ToString();

                GetPrivateProfileString(pAppNm, "SC_GRP_NO", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pScGrpNo = sb.ToString();

                pPortCnt = GetPrivateProfileInt(pAppNm, "PORT_CNT", pPortCnt, cDefApp.GM_ENV_INI);

                GetPrivateProfileString(pAppNm, "LOG_PATH", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pLogPath = sb.ToString();

                GetPrivateProfileString(pAppNm, "FILENAME", null, sb, sb.Capacity, cDefApp.GM_ENV_INI);
                pLogFileNm = sb.ToString();

                pRtnMsg = "[GsReadInitProfileCom]::Sucess";
                return true;
            }
            catch (Exception ex)
            {
                pRtnMsg = ex.Message;
            }
            pRtnMsg = "[GsReadInitProfileCom]Error::" + pRtnMsg;
            return false;
        }
        #endregion

    }
}
