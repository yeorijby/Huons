using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;

namespace WCS_TASK_SC
{  
    class cDbUse : cDbBaseOra   //@.���
    {
         //@. �Ϲ����� PC Client���� Global Connection ��ü 1�� ��� ��
        public cDbUse()
        {
            //(GM_CN_DB, pBind);
            this.DbBaseOra(ref cDefApp.GM_DB1_CN, false);
        }
        public  cDbUse(bool pBind)
        {
            //(GM_CN_DB, pBind);
            this.DbBaseOra(ref cDefApp.GM_DB1_CN, pBind);
        }

    //@. cBaseDb���� Connection�� ������ �������� �� ���, PDA Server
    //@. Backgroud Process���� cBaseDb�� Connection��ü�� ��� �� ���
    //@. strDummy�� Overload �Լ� ���� �� �����ϱ� ���� �Ķ����, �� �ǹ̰� ����.
    //@. ex) Public BDb As New cDbUse("Multi", False)
    public  cDbUse(string pStrDummy, bool pBind )
    {
        this.DbBaseOra(pBind); //@.Backgroud Process���� CBaseDb�� Connection��ü�� ��� �� ���(//@. ����� comMain.Close�� �ݵ�� ȣ��)
    }

    //@. Connection�� 2�� �̻��� ��� (����), ������ ������ �ʿ��� ��
    public  cDbUse(ref OleDbConnection  pCon, bool pBind)
    {
        this.DbBaseOra(ref pCon, pBind);
    }

    ////@. �޼��� ���̱� ������ �Ͽ� ��� �� ��
    //    public static new void ShowErrMsg(bool pMsgBox)
    //    {
    //        //@. �޼��� �ٲٱ�
    //        switch (DB_LOCK)
    //        {
    //            case DB_LOCK :ErrMsg = "��� �� �ٽ� ����ϼ���. (DB Lock.)";
    //            case DB_DUP:ErrMsg = "�̹� ��ϵ� �ڷ��Դϴ�. (DB �ߺ�)";
    //        }

    //        //@.//@. �޼����ڽ� ���̱⸦ ���� �� ���
    //        //@.If bMsgBox Then
    //        //@.    ShowMsgClient(ErrMsg, "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    //        //@.End If
    //    }
    }
}
