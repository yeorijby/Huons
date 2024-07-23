using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WCS_TASK_SC
{
    public class maindefine
    {
        public string BytesToHexs(byte[] value, int len)
        {
            string cvtvalue = "";
            for (int ii = 0; ii < len; ii++)
            {
                cvtvalue = cvtvalue + string.Format("{0:X2}", value[ii]);
                if (((ii + 1) % 2) == 0) cvtvalue = cvtvalue + " ";
            }
            return cvtvalue;
        }

		public string BytesToHexs(byte[] value, int nStartLen, int nEndlen)
		{
			string cvtvalue = "";
			for (int ii = nStartLen; ii < nEndlen; ii++)
			{
				cvtvalue = cvtvalue + string.Format("{0:X2}", value[ii]);
				if (((ii + 1) % 2) == 0) cvtvalue = cvtvalue + "";
			}
			return cvtvalue;
		}



		/*
		 * [Method]Collection ��ü���� ��Ʈ���� ã�� �����Ѵ�.
		 */
        #region
        public Control PfCtlFind(ref Panel pPnl, string pCtlNm, ref string pRtnMsg)
        {
            Control[] ctl;

            try
            {
                pRtnMsg = "";

                ctl = pPnl.Controls.Find(pCtlNm, true);

                if (ctl.Length == 0)
                {
                    return null;
                }
                else
                {
                    pRtnMsg = "[PfCtlFind]Success::" + "(" + pCtlNm + ")";
                    return ctl[0];
                }
            }
            catch (Exception ex)
            {
                pRtnMsg = ex.Message;
            }
            pRtnMsg = "[PfCtlFind]Error::" + "(" + pCtlNm + ")" + pRtnMsg;
            return null;
        }

        public Control PfCtlFindTab(ref TabControl pPnl, string pCtlNm, ref string pRtnMsg)
        {
            Control[] ctl;

            try
            {
                pRtnMsg = "";

                ctl = pPnl.Controls.Find(pCtlNm, true);

                if (ctl.Length == 0)
                {
                    return null;
                }
                else
                {
                    pRtnMsg = "[PfCtlFind]Success::" + "(" + pCtlNm + ")";
                    return ctl[0];
                }
            }
            catch (Exception ex)
            {
                pRtnMsg = ex.Message;
            }
            pRtnMsg = "[PfCtlFind]Error::" + "(" + pCtlNm + ")" + pRtnMsg;
            return null;
        }
        #endregion
    }
}
