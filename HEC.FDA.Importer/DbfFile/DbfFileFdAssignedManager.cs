using System;
using System.Collections.Generic;
using System.Text;

namespace Importer
{
    class DbfFileFdAssignedManager
    {
        public enum AssignDataFileSaveType { NEW, RENAME, SAVE, SAVEAS, UNKNOWN };

        //m_classType - Handles distinctions for setting results as invalid.
        /*
          WSP
          PROB
          RATE
          LEVEE
          AGDAMG
          EAD
          EQEAD
        */
        string m_classType;


        //m_typeLookupPYSRC - Type of assigned record (plan, plan & year,
        //plan & year & stream, etc.
        char m_typeLookupPYSRC;

        long m_idPlan;
        long m_idYear;
        long m_idStream;
        long m_idReach;
        long m_idCategory;

        long m_idFuncNew;
        string m_nameFuncNew;
        string m_name;
        long m_sortOrder;

        int m_replaceGlobal; //Option set for import; Maybe also for default reply in GUI?
        int m_mustAppendLook;
        int m_mustReplaceLook;
        int m_mustAppendData;
        int m_mustReplaceData;
        int m_mustDecrementNumRef;
        int m_mustIncrementNumRef;

        int m_appendAllowed;
        int m_replaceLocalRequired;

        //Data related to status of names & ID's in look and data files & user's
        //  request to either replace global or local.
        int m_replaceGlobalByUser;
        int m_appendNewByUser;
        int m_duplicateByUser; //GUI not setup for this; this is a make change, save local
        int m_renameFunc;

        int m_nameMatchUsrToData; //User supplied name matches one already in data file.
        int m_replaceDataNoLook;
        string m_nameByUser;  //From GUI or Import File


        //Actions - Data Record
        int m_functionNew;
        int m_functionRename;
        int m_functionSave;
        int m_functionSaveAs;

        AssignDataFileSaveType m_AssignDataFileSaveType;

        //Status of Data in Files
        int m_lookExists; //look record exists for selected pysrc
        int m_dataExists; //data exists for ID that pysrc points to
        int m_dataUserNameExists;

        long m_recNumLook;
        long m_recNumData;
        long m_recNumDataUserName;
        string m_nameInData;
        long m_idInLook;
        long m_idFuncOrig;
        long m_idFuncUserName;
        long m_numReferences;


        //should these be void pointers? Then defined by sub-classes?
        //Maybe not. It would at least require a lot of changes
        //rdc critical;rdc current;10Jan08

        //Database Pointers
        public DbfFileFdDataLookManager mp_dbfData;
        public DbfFileFdLookupManager mp_dbfLook;
        public NameManager mp_nameManager; //Pointer to real object in DbfFileFdDataManager

        //-------------------------------------------------------------------------------
        public DbfFileFdAssignedManager()
        {
          m_classType    = null;
          mp_dbfData     = null;
          mp_dbfLook     = null;
          mp_nameManager = null;
        }
        void reset()
        {
            m_idFuncOrig = m_idFuncNew = m_sortOrder = -1L;
            m_numReferences = 0;
            m_recNumLook = m_recNumData = -1L;

            m_mustAppendLook = m_mustReplaceLook = 0;
            m_mustAppendData = m_mustReplaceData = m_mustDecrementNumRef = m_mustIncrementNumRef = 0;
        }

    }
}


