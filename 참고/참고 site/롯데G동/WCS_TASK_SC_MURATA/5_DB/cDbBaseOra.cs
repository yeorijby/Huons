using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace WCS_TASK_SC
{
    public class cDbBaseOra
    {
        #region@@@.���� ��ü����[OleDb]
        public  OleDbConnection mCnMain;                                // @.�ܺο��� �Ҵ����ִ� Connection ��ü ����(Connection�� �ϳ� �� ���)
        public OleDbTransaction mTrnMain;                              // @.��ü �����Ǵ� DB ��ü��
        public OleDbCommand mComMain = new OleDbCommand();             // @.��ü �����Ǵ� DB ��ü��

        public bool mIsBeginTran = false;                             // @.Begin Tran ����
        public OleDbDataAdapter mDaMain = new OleDbDataAdapter();      // @.Data Adapter
        public bool mBindingType;                                      // @.���ε� ��ü�� ������� ����[���ε� ��ü�� ��� Reset�� �ϸ� �ȵ�.(��� ����� ����, DataSource�� ���� �۵�)]                                 // @.SQL Command
        #endregion

        #region@@@.��� ����
        public const int DB_ERR  = -1;      // @.DB ����
        public const int DB_LOCK = -2;      // @.DB ������ DB Lock
        public const int DB_DUP = -3;       // @.DB ������ �ߺ� ����Ÿ
        #endregion

        #region @@@.�Ӽ�[DtMain]
        public DataTable mDtMain = new DataTable("Default");            // @.��ü �����Ǵ� DB ��ü��
        public DataTable DtMain
        {
            get
            {
                return DtMain;
            }
        }
        #endregion

        #region @@@.�Ӽ�[DbConnted]
        public bool mDbConnted = false;    // @.DB Connection ��������
        public bool DbConnted
        {
            get
            {
                return mDbConnted;
            }
            set
            {
                mDbConnted = value;
            }
        }
        #endregion

        #region @@@.�Ӽ�[ErrMsg]
        private string mErrMsg = "";      // @.DB Error Message
        public string ErrMsg 
        {
          get {
            return mErrMsg;
          }
          set {
            mErrMsg = value;
          }
        }
        #endregion

        #region @@@.�Ӽ�[ErrKind]
        private int mErrKind = 0;         // @.DB Error ����
        public int ErrKind 
        {
          get {
            return mErrKind;
          }
        }
        #endregion

        #region @@@.������
        // @.��ü Connection ��ü ���
        // @.�ܺο��� New ���� �� Connection ��ü�� Open�ϰ� Init�� ȣ���Ѵ�.
        // @.����� mComMain.Close�� �ݵ�� ȣ��
        public cDbBaseOra() 
        {
            this.mBindingType = false; 
        }

        public cDbBaseOra(bool bBind) 
        {
            this.mBindingType = bBind; 
        }

        public cDbBaseOra(ref OleDbConnection pConObj) 
        {
            this.mBindingType = false;
            this.mCnMain = pConObj;
            this.Init();
        }

        public cDbBaseOra(ref OleDbConnection pConObj, bool bBind) 
        {
            this.mBindingType = bBind;
            this.mCnMain = pConObj;
            this.Init();
        }
        #endregion

        public void DbBaseOra()
        {
            this.mBindingType = false;
        }

        public void DbBaseOra(bool bBind)
        {
            this.mBindingType = bBind;
        }

        public void DbBaseOra(ref OleDbConnection pConObj)
        {
            this.mBindingType = false;
            this.mCnMain = pConObj;
            this.Init();
        }

        public void DbBaseOra(ref OleDbConnection pConObj, bool bBind)
        {
            this.mBindingType = bBind;
            this.mCnMain = pConObj;
            this.Init();
        }

        #region@@@.DB init
        public void Init()
        {
            mComMain.Connection = mCnMain;
            mComMain.CommandType = CommandType.Text;
            mDaMain.SelectCommand = mComMain;
            mDbConnted = true;
            // ���� �뵵�� None query�� ��� ���� DataCommand�� ����.
            //        comNonSel.Connection = gconDb
            //       comNonSel.CommandType = CommandType.Text
        }
        #endregion

        #region@@@.DB Error Message[������Ʈ ���� �޼����� ǥ���ϴ� ����� �л� Ŭ�������� �������̵� �ؼ� ����Ѵ�.]
        //* ������Ʈ ���� �޼����� ǥ���ϴ� ����� �л� Ŭ�������� �������̵� �ؼ� ����Ѵ�.
        public static void ShowErrMsg(bool  bMsgBox)
        {
            if (bMsgBox == true)
            {
                ////MessageBox.Show(ErrMsg, "DB Err||", MessageBoxButtons.OK, MessageBoxIcon.Err||)
            } 
        }
        #endregion

        #region@@@.���� ���� For Select..  
        // @@.Parameter
        // @.pIsMsgBox (�޼��� �ڽ� ǥ�� ����)
        // @.pIsRtnErr (���� �߻���, ������ Return�� �� ����)
        // @@.Return
        // @.���� - ������ ���ڵ� �� (���� ����)
        // @.���� - DB_ERR(-1):  �Ϲ� DB Err
        // @.���� - DB_LOCK(-2): DB Lock
        // @.���� - DB_DUP(-3):  ����Ÿ �ߺ�
        public int ExcuteQry(string  strQry) 
        {
            return ExcuteQry( strQry, true , false);
        }
        public int ExcuteQry(string  strQry, bool bMsgBox) 
        {
            return ExcuteQry( strQry, bMsgBox, false);
        }
        public int ExcuteQry(string  strQry  , bool bMsgBox, bool breturnErr) 
        {
            mErrKind = 0;
            try
            {
                // ���ε� ��ü�� ��� ��������, DATA�� Ŭ����
                if (mBindingType == true)
                {
                    mDtMain.Clear();
                } 
                else 
                {
                    mDtMain.Reset();
                }
                mComMain.CommandText = strQry;
                return mDaMain.Fill(mDtMain);
            }
            catch (OleDbException DbErr)
            {
                if (breturnErr) 
                {
                    throw DbErr;
                }
                else 
                {
                    mErrMsg = DbErr.Message;
                    if (mErrMsg.IndexOf("||A-03114") != -1 || mErrMsg.IndexOf("||A-12560") != -1)
                    {
                        mDbConnted = false ;
                    }  
                    
                    if (mErrMsg.IndexOf("||A-00054") != -1 )
                    {
                        // No wait �� ����� ���
                        mErrKind = DB_LOCK;
                    }
                    else
                    {
                        mErrKind = 0;
                        ////ShowErrMsg(bMsgBox);
                    }
                }
            return DB_ERR;
            }
        }
        #endregion

        #region@@@.���� ���� For Select..  [2�� �̻� ������ �� ��� datatable�� ������ ���ε� �Ѵ�.]
        // @@.Parameter
        // @.bRerutnErr[���� �߻���, ������ Return�� �� ����]
        // @@.Return
        // @.���� - ������ ���ڵ� �� (���� ����)
        // @.���� - DB_ERR(-1):  �Ϲ� DB Err
        // @.���� - DB_LOCK(-2): DB Lock
        // @.���� - DB_DUP(-3):  ����Ÿ �ߺ�
        public int ExcuteQry(ref DataTable dtOther, string  strQry)  
        {
            return ExcuteQry(ref dtOther, strQry, true, false);
        }
        public int ExcuteQry(ref DataTable dtOther, string  strQry, bool bMsgBox)  
        {
            return ExcuteQry(ref dtOther, strQry, bMsgBox, false);
        }
        public int ExcuteQry(ref DataTable dtOther, string  strQry, bool bMsgBox, bool breturnErr)  
        {
            mErrKind = 0;
            try
            {
            
                // ���ε� ��ü�� ��� ��������, DATA�� Ŭ����
                if (mBindingType == false ) 
                {
                    dtOther.Clear();
                }
                else
                {
                    dtOther.Reset();
                }

                mComMain.CommandText = strQry;
                return mDaMain.Fill(dtOther);
            }
            catch( OleDbException DbErr)
            {
                if (breturnErr == true ) 
                {
                    throw DbErr;
                }
                else
                {
                    mErrMsg = DbErr.Message;

                    if (mErrMsg.IndexOf("||A-03114") != -1 || mErrMsg.IndexOf("||A-12560") != -1) 
                    { 
                        mDbConnted = false;
                    }

                    if (mErrMsg.IndexOf("||A-00054") != -1) 
                    {
                        // No wait �� ����� ���
                        mErrKind = DB_LOCK;
                    }
                    else
                    {
                        mErrKind = 0;
                    }
                    ////ShowErrMsg(bMsgBox)
                }
            }
            return DB_ERR;
        }
        #endregion
        
        #region@@@.None Query For insert, update, ...
        // @@.Parameter
        // @.bRerutnErr[���� �߻���, ������ Return�� �� ����]
        // @@.Return
        // @.���� - �ݿ��� ���ڵ� �� (���� ����)
        // @.���� - DB_ERR(-1):  �Ϲ� DB Err
        // @.���� - DB_LOCK(-2): DB Lock
        // @.���� - DB_DUP(-3):  ����Ÿ �ߺ�
        public int  ExcuteNonQry(string  strQry) 
        {
            return ExcuteNonQry(strQry, true, false);
        }
        public int ExcuteNonQry(string strQry, bool bMsgBox)
        {
            return ExcuteNonQry(strQry, bMsgBox, false);
        }
        public int  ExcuteNonQry(string  strQry, bool bMsgBox, bool breturnErr) 
        {
            mErrKind = 0;
            try
            {
                mComMain.CommandText = strQry;
                return mComMain.ExecuteNonQuery();
            }
            catch(OleDbException DbErr)
            {
                if (breturnErr == true) 
                {
                    throw DbErr;
                }
                else
                {
                    mErrMsg = DbErr.Message;
                    if (mErrMsg.IndexOf("||A-03114") != -1 || mErrMsg.IndexOf("||A-12560") != -1) 
                    { 
                        mDbConnted = false;
                    }

                    if (mErrMsg.IndexOf("||A-00001") != -1)
                    {
                        mErrKind = DB_DUP;
                    }
                    else
                    {
                        mErrKind = 0;
                    }
                    ////ShowErrMsg(bMsgBox)
                }
            }
            return DB_ERR;
        }
        #endregion
        
        #region@@@.Transction ��ü �Ҵ�
        public void BeginTrans()
        {
            try
            {
                    mTrnMain = mCnMain.BeginTransaction();
                mIsBeginTran = true;
            }
            
            catch (Exception AppErr)
            {
                throw AppErr;
            }
        }
        #endregion

        #region@@@.Transction Commit
        public void Commit()
        {
            try
            {
                if (mIsBeginTran == true) 
                {
                    mTrnMain.Commit();
                    mIsBeginTran = false;
                }
                
            }
            catch (Exception AppErr)
            {
                throw AppErr;
            }
        }
        #endregion

        #region@@@.Transction Rollback
        public void Rollback()
        {
            try
            {
                if (mIsBeginTran == true)
                {
                    mTrnMain.Rollback();
                    mIsBeginTran = false;
                }
            }
            catch(Exception AppErr)
            {
                throw AppErr;
            }
        }
        #endregion
        
        #region@@@.���ν��� ����� ��� ����
        public void SetProcedure(string  pSp_Name)
        {
            mComMain.CommandType = CommandType.StoredProcedure;
            mComMain.CommandText = pSp_Name;
            mComMain.Parameters.Clear();
        }
        #endregion
    }
}
