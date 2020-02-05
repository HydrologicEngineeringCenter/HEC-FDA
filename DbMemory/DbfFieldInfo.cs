using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;

namespace DbMemory
{
    // DbfFieldInfo.h
    /*
    ================================================================================
      Classes to handle information for dbf files:
        1)  create field & tag information
        2) Read existing dbf file for field and tag information
        3) Read / Write data from/to dbf file

      Uses linked lists to that one field is defined as an element of a linked list.
        the DbfFieldInfo is a LINK4 object. The DbfFileManager object contains a
        LIST4 control object for positioning the LINK4 object at a desired location.
    ================================================================================

      Used for setting / defining? the field definition data for one fieldname.
        mp_field4Info must point to a field.
    */



    //==============================================================================
    //class DbfFieldInfo : public LINK4
    public class DbfFieldInfo
    {

        //------------------------------------------------------------------------------
        //Variables

        //............................................................................
        //CodeBase Related Information
        //FIELD4*    mp_field4;
        //FIELD4INFO m_field4Info;

        string m_fieldName;

        //............................................................................
        /* Types normally used for Function/Operator parameters and returns
        #define  r4date     'D'
        #define  r4dateDoub 'd'
        #define  r4float    'F'
        #define  r4log      'L'
        #define  r4num      'N'
        #define  r4numDoub  'n'
        #define  r4str      'C'
        #define  r4memo     'M'
        #define  r4gen      'G'
        //............................................................................*/
        //short int          m_type; //'C', 'D', 'F', 'L', 'M', 'N', 'B' and 'G'.
        //                           //67   68   70   76   77   78   66      71
        //char               m_typeC;
        char m_dataType; //r4num, r4float, r4str, r4memo, r4date, f4log, r4memo_binary
        int m_len;
        int m_dec;

        //Need to implement checking with this to make sure we don't mix types.
        //rdc critical;rdc current;01Jul08
        int m_memoStyle; // 0=string; 1=tab delimt; 2=legacy binary

        //Information for cross-referencing data to FIELD4INFO
        string m_dataDescriptor;

        //Data for database update
        int m_pointerToOldField;
        int m_pointerToOldFile; //For use when updating 2 old files into 1 new file.

        //............................................................................
        //Data in this field for current record.

        long m_longValue;
        char m_logicalValue;
        double m_doubleValue;

        int m_numCharMemo;
        string mp_memoValue;

        long m_numValues; //if > 0, used to copy doubles to / from memo field.
        long m_blockSize; //used to copy legacy data to/from memo fields.
        long m_numRows;
        long m_numCols;
        long m_ixColumn;
        double[,] mp_doubleValues;
        long[,] mp_longValues;

        //Methods
        public DbfFieldInfo() //set to void
        {
            //mp_field4   = null;
            mp_memoValue = null;
            mp_doubleValues = null;
            mp_longValues = null;
            //m_field4Info.name = NULL;
            //m_field4Info.type = r4num;
            //m_field4Info.len  = 10;
            //m_field4Info.dec  = 0;
            mp_memoValue = null;

            setDefault();
        }
        public DbfFieldInfo(string fieldName)  //set to default except for name
        {
            //this->mp_field4   = NULL;
            mp_memoValue = null;
            mp_doubleValues = null;
            mp_longValues = null;
            //m_field4Info.name = NULL;
            //m_field4Info.type = r4num;
            //m_field4Info.len  = 10;
            //m_field4Info.dec  = 0;
            mp_memoValue = null;

            setDefault();
            setNameField(fieldName);
        }
        public DbfFieldInfo(string fieldName,   //set to default except for name
                            int type,
                            int len,
                            int dec,
                            string dataDescriptor,
                            int memoStyle)
        {
            //this->mp_field4 = NULL;
            mp_memoValue = null;
            mp_doubleValues = null;
            mp_longValues = null;
            //m_field4Info.name = NULL;
            //m_field4Info.type = r4num;
            //m_field4Info.len = 10;
            //m_field4Info.dec = 0;
            mp_memoValue = null;

            setFieldInfo(fieldName,
                               type,
                               len,
                               dec,
                               dataDescriptor,
                               memoStyle);
        }

        //Field Definition Information
        void setDefault()
        {
            m_fieldName = string.Empty;
            m_dataDescriptor = string.Empty;

            m_len = 10;
            m_dec = 0;
            m_memoStyle = 0;

            m_pointerToOldField = -1;
            m_pointerToOldFile = -1;

            //m_dataType = r4num;
            m_longValue = 0;
            m_logicalValue = 'T';
            m_doubleValue = 0.0;
            m_numCharMemo = 0;
            m_numValues = 0;
            m_blockSize = 0;
            m_numRows = m_numCols = 0;
            m_ixColumn = 0;

            mp_memoValue = null;
            mp_doubleValues = null;
            mp_longValues = null;
        }
        public void clone(DbfFieldInfo oldDbfFieldInfo)
        {
            DbfFieldInfo o = oldDbfFieldInfo;

            //this->mp_field4 = o->getField4();

            setFieldInfo(o.m_fieldName,
                               o.m_dataType,
                               o.m_len,
                               o.m_dec,
                               o.m_dataDescriptor,
                               o.m_memoStyle);

            this.m_longValue = o.m_longValue;
            this.m_logicalValue = o.m_logicalValue;
            this.m_doubleValue = o.m_doubleValue;

            if (o.mp_memoValue != null)
            {
                int nc = o.mp_memoValue.Length;
                mp_memoValue = o.mp_memoValue;  //rdc critical;!TODO;need to make a new copy?
                m_numCharMemo = nc;
            }
            else
            {
                this.mp_memoValue = null;
                m_numCharMemo = 0;
            }
            this.m_pointerToOldField = o.m_pointerToOldField;
            this.m_pointerToOldFile = o.m_pointerToOldFile;
            this.m_numValues = o.m_numValues;
            this.m_blockSize = o.m_blockSize;

            if (this.m_blockSize > 0)
            {
                //this.mp_memoValue = new char[m_blockSize + 1];
                //memset(mp_memoValue, '\0', m_blockSize + 1);
                //memcpy(mp_memoValue, o.mp_memoValue, m_blockSize);
                mp_memoValue = o.mp_memoValue;  //rdc critical;!TODO;need to make a new copy?
            }
        }
        //void           setField4(FIELD4* field4);
        // FIELD4*        getField4();

        public void setFieldInfo(string fieldName,
                                    int type,
                                    int len,
                                    int dec,
                                    string dataDescriptor,
                                    int memoStyle = 0)
        {
            m_fieldName = string.Empty;
            if (fieldName != null)
            {
                if (fieldName.Length > 0)
                    m_fieldName = fieldName.TrimEnd().PadRight((int)GlobalVariables.SIZE_DBF_CHAR.DBF_FIELDNAME_LENGTH);
            }
            //this.m_type = type;
            this.m_dataType = (char)type;
            this.m_len = len;
            this.m_dec = dec;
            this.m_memoStyle = memoStyle;

            m_dataDescriptor = string.Empty;

            if (dataDescriptor != null)
            {
                if (dataDescriptor.Length > 0)
                {
                    m_dataDescriptor = dataDescriptor.TrimEnd().PadRight((int)GlobalVariables.SIZE_DBF_CHAR.FIELD_DESCRIPTOR_SIZE - 1);
                }
            }
            else
            {
                m_dataDescriptor = " ";
            }
        }
        /*
         * !TODO; rdc critical;19Dec2018
        void setFieldInfo(FIELD4INFO_BOB f4ibob)
        {
            setFieldInfo(f4ibob.name,
                     f4ibob.type,
                     f4ibob.len,
                     f4ibob.dec,
                     f4ibob.dataDescriptor,
                     f4ibob.memoStyle);
        }
        */
        //void           setFieldInfo(FIELD4INFO f4i, char *dataDescriptor);

        public void setFieldInfo(string fieldName,
                                    int type,
                                    int len,
                                    int dec)
        {
            this.setDefault();

            m_fieldName = string.Empty;
            if (fieldName != null)
            {
                if (fieldName.Length > 0)
                    m_fieldName = fieldName.TrimEnd().PadRight((int)GlobalVariables.SIZE_DBF_CHAR.DBF_FIELDNAME_LENGTH);
            }
            this.m_dataType = (char)type;
            this.m_len = len;
            this.m_dec = dec;
        }
        //void           setFieldInfo(FIELD4INFO f4i);
        //FIELD4*        mapField(DATA4* data4);
        public string getNameField()
        {
            return m_fieldName;
        }
        public void setNameField(string name)
        {
            m_fieldName = name.Trim().PadRight((int)GlobalVariables.SIZE_DBF_CHAR.DBF_FIELDNAME_LENGTH);
        }
        public char getFieldType()
        { return m_dataType; }
        public void setFieldType(char c)
        { m_dataType = c; }

        public int getFieldLength()
        {
            return m_len;
        }
        public void setFieldLength(int len)
        {
            m_len = (int)Math.Max(0, len);
        }

        public int getFieldDecimal()
        {
            return m_dec;
        }
        public void setFieldDecimal(int dec)
        {
            m_dec = (int)Math.Max(0, dec);
        }

        public int getMemoStyle()
        {
            return m_memoStyle;
        }
        public void setMemoStyle(int memoStyle)
        {
            m_memoStyle = memoStyle;
        }

        public void setBlockSize(long blockSize)
        {
            m_blockSize = blockSize;
        }
        public long getBlockSize()
        {
            return m_blockSize;
        }

        //void           readOneFieldInfo(DATA4* mp_data4, int ixField);

        public void printOneFieldInfo(ref StreamWriter w,
                                        int indexField,
                                        int[] ncWidEachVariable,
                                        int nIncludeLineFeed,
                                        int tIncludeTagAtLineEnd)
        {
            printOneFieldInfo(ref w,
                  indexField,
                  m_fieldName,
                  m_dataType,
                  m_len,
                  m_dec,
                  ncWidEachVariable,
                  nIncludeLineFeed,
                  tIncludeTagAtLineEnd);

        }

        //public void printOneFieldInfo( ofstream &w,
        //                                int indexField,
        //                                FIELD4INFO f4i,
        //                                int ncWidEachVariable[],
        //                                int nIncludeLineFeed,
        //                                int tIncludeTabAtLineEnd);

        //public void printOneFieldInfo( ofstream &w,
        //                                int indexField,
        //                                FIELD4INFO f4i);


        public void printOneFieldInfo(ref StreamWriter w,
                                        int indexField,
                                        string name,
                                        int type,
                                        int len,
                                        int dec,
                                        int[] ncWidEachVariable,
                                        int nIncludeLineFeed,
                                        int tIncludeTabAtLineEnd)
        {
            string data, format;

            for (int i = 0; i < 5; i++)
            {
                format = string.Empty;
                data = string.Empty;

                switch (i)
                {
                    case 0:
                        //strcpy(format, "%7d %s");
                        //sprintf(data, format, indexField + 1, " ");
                        data = (indexField + 1).ToString("       ");
                        break;
                    case 1:
                        //copyPadded(data, name, ncWidEachVariable[i]);
                        //strcat(data, "     ");
                        data = name.Trim().PadRight(ncWidEachVariable[i]);
                        data += "     ";
                        break;
                    case 2:
                        //format[0] = (char)type;
                        //copyPadded(data, format, ncWidEachVariable[i]);
                        //strcat(data, "    ");
                        format = type.ToString(" ");
                        data = format.Trim().PadRight(ncWidEachVariable[i]);
                        data += "    ";
                        break;
                    case 3:
                        //strcpy(format, "%");
                        //sprintf(&format[1], "%1d", ncWidEachVariable[i]);
                        //strcat(format, "d %s");
                        //sprintf(data, format, len, "    ");
                        for (int j = 0; j < ncWidEachVariable[i]; j++)
                            format += " ";
                        data = len.ToString(format);
                        data += "    ";
                        break;
                    case 4:
                        //strcpy(format, "%");
                        //sprintf(&format[1], "%1d", ncWidEachVariable[i]);
                        //strcat(format, "d %s");
                        //sprintf(data, format, dec, "    ");
                        for (int j = 0; j < ncWidEachVariable[i]; j++)
                            format += " ";
                        data = dec.ToString(format);
                        data += "    ";
                        break;
                }
                w.Write(data);
            }
            if (tIncludeTabAtLineEnd > 0) w.Write('\t');
            if (nIncludeLineFeed > 0) w.Write("\n");
        }
        public void printOneFieldInfo(ref StreamWriter w,
                                        int indexField,
                                        string name,
                                        int type,
                                        int len,
                                        int dec)
        {
            w.WriteLine($"{indexField + 1} \t{name} \t{type.ToString(" ")} \t{len} \t {dec} \t");
        }

        public void setDataDescriptor(string descriptor)
        {
            m_dataDescriptor = descriptor.Trim().PadRight((int)GlobalVariables.SIZE_DBF_CHAR.FIELD_DESCRIPTOR_SIZE - 1);

        }
        public string getDataDescriptor()
        {
            return m_dataDescriptor;
        }

        public void setPointerToOld(int ixField)
        {
            m_pointerToOldField = ixField;
        }
        public int getPointerToOld()
        {
            return m_pointerToOldField;
        }

        public void setPointerToOldFile(int ixFile) { m_pointerToOldFile = ixFile; }
        public int getPointerToOldFile() { return m_pointerToOldFile; }

        //............................................................................
        //Data methods
        //............................................................................
        void resetData()
        {
            /*
              Reset the data portion so that old data isn't processed.
            */
            mp_memoValue = null;
            mp_doubleValues = null;
            mp_longValues = null;

            m_longValue = 0L;
            m_logicalValue = 'T';
            m_doubleValue = Study.badNumber;

            m_numCharMemo = 0;
            mp_memoValue = null;

            m_numValues = 0;
            m_blockSize = 0;
            m_numRows = 0;
            m_numCols = 0;
            mp_doubleValues = null;
            mp_longValues = null;
        }

        //set methods
        void setValue(int v)
        {
            setValueLong((long)v);
            m_numValues = 0;
        }
        void setValue(long v)
        {
            setValueLong(v);
        }
        void setValue(double v)
        {
            setValueDouble(v);
        }
        public string setValue(string v)
        {
            /*
              Straight storage of character string
            */
            string vs = null;
            if (v != null)
            {
                if (mp_memoValue != null)
                {
                    if (v == mp_memoValue)
                    {
                        //char* vs = new char[strlen(v) + 1];
                        //strcpy(vs, v);
                        //this->setValueMemo(vs);
                        vs = v;  //TODO; create new object?
                        setValueMemo(vs);
                    }
                    else
                    {
                        setValueMemo(v);
                    }
                }
                else
                {
                    setValueMemo(v);
                }
            }
            else
            {
                setValueMemo("");
            }
            return vs;
        }

        public void setValue(double[,] v, long numRows, long numCols = 1)
        {
            /*
              This stores an array of doubles in the DbfFieldInfo object. v can have
              more than one column.
            */
            if (numRows > 0 && numCols > 0)
            {
                double[,] lpv = new double[numRows , numCols];
                //for (int i = 0; i < numRows * numCols; i++) lpv[i] = v[i];
                for(int ir = 0; ir < numRows; ir++)
                {
                    for(int ic = 0; ic < numCols; ic++)
                    {
                        lpv[ir, ic] = v[ir, ic];
                    }
                }

                if (m_memoStyle == 1)
                {
                    setValueMemo(lpv, numRows, numCols);
                }
                else
                {
                    setValueMemoLegacy(v, numRows, numCols);
                }
                lpv = null;
            }

        }
        public void setValue(long[,] vl, long numRows, long numCols = 1)
        {
            /*
              This stores an array of longs in the DbfFieldInfo object. vl can have
              more than one column.
            */
            setValueMemo(vl, numRows, numCols);
        }

        void setValueLong(long v)
        {
            m_longValue = v;
            m_numValues = 0;
            m_dataType = 'N';
            m_blockSize = 0;
        }
        void setValueLogical(char v)
        {
            if (v == 'T' || v == 't' || v == 'Y' || v == 'y')
            {
                m_logicalValue = 'T';
            }
            else
            {
                m_logicalValue = 'F';
            }
            m_numValues = 0;
            m_dataType = 'L';
            m_blockSize = 0;

        }
        void setValueDouble(double v)
        {
            m_doubleValue = v;
            m_numValues = 0;
            m_dataType = 'F';
            //m_dataType = r4numDoub;
            m_blockSize = 0;

        }

        void setNumRows(int numRows)
        {
            m_numRows = numRows;
        }

        void setNumCols(int numCols)
        {
            m_numCols = numCols;
        }
        public void setNumRowsAndCols(int numRows, int numCols = 1)
        {
            /*
              Resets the data, sets the number of rows and columns, allocates memory
            */
            resetData();
            m_numRows = numRows;
            m_numCols = numCols;
            m_ixColumn = 0;

            mp_doubleValues = null;

            m_numValues = m_numRows * m_numCols;
            //mp_doubleValues = new double[m_numValues];
            //for (int i = 0; i < m_numValues; i++) mp_doubleValues[i] = Study.badNumber;
            mp_doubleValues = new double[m_numRows, m_numCols];
            for (int i = 0; i < m_numRows; i++)
            {
                for (int j = 0; j < m_numCols; j++)
                {
                    mp_doubleValues[i, j] = Study.badNumber;
                }
            }
        }

        public void setValueMemo(string v)
        {
            /*
              This stores a character string.

              Normally used to store a simple character string.
              However, it is also used to store a tab-delimited string of numbers that
                has been created using MemoFieldProcessing. But, you also need to set the
                number of rows and columns.
            */

            int nc = v.Length;

            if ((mp_memoValue != null && v != mp_memoValue) || mp_memoValue == null)
            {
                //if (mp_memoValue) delete[] mp_memoValue;
                //mp_memoValue = new char[nc + 1];
                //memset(mp_memoValue, '\0', nc + 1);
                //strcpy(mp_memoValue, v);
                mp_memoValue = v.TrimEnd();
            }
            m_numCharMemo = mp_memoValue.Length;
            m_blockSize = 0;
        }


        public void setValueMemo(double[,] v, long numRows, long numCols)
        {
            /*
              This stores an array of doubles in the DbfFieldInfo object as a tab-
                delimited character string.

              v can have more than one column.
              It sets both the double array as well as the conversion to a tab delimited
                memo string.
            */

            int i = 0;
            long numValues = numRows * numCols;
            double[,] lpv = null;

            m_blockSize = 0;

            if (numValues > 0)
            {
                m_numValues = numRows * numCols;
                m_numRows = numRows;
                m_numCols = numCols;

                //lpv = new double[numValues];
                lpv = new double[numRows, numCols];

                //for (i = 0; i < numValues; i++) lpv[i] = v[i];
                for (i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        lpv[i, j] = v[i, j];
                    }
                }

                //Create the tab-delimited memo string
                //MemoFieldProcessing* mfp = new MemoFieldProcessing();
                MemoDataField mdf = new MemoDataField();

                //string s = mfp->dataToMemoString(lpv, numRows, numCols);
                string sm = string.Empty;
                MemoDataField.ProcessDoubleToMemoString((int)numRows, (int)numCols, lpv, ref sm);
                //setValueMemo(s);
                setValueMemo(sm);

                //Store the double array
                //if (mp_doubleValues) delete[] mp_doubleValues;
                mp_doubleValues = null;

                //mp_doubleValues = new double[m_numValues];
                mp_doubleValues = new double[m_numRows, m_numCols];
                //for (i = 0; i < m_numValues; i++) mp_doubleValues[i] = lpv[i];
            }
            else
            {
                m_numRows = 0;
                m_numCols = 0;
                m_numValues = 0;

                //rdc 4Aug2010;Repeated use, old values existed in memo field
                resetData();
            }
            //if (lpv) delete[] lpv;
            lpv = null;

        }
        void setValueMemo(long[,] vl, long numRows, long numCols)
        {
            /*
              This stores an array of longs in the DbfFieldInfo object as a tab-
                delimited character string.

              vl can have more than one column. It sets both the long array as well as the
                conversion to a tab delimited memo string.
            */

            m_numValues = numRows * numCols;
            m_numRows = numRows;
            m_numCols = numCols;
            m_blockSize = 0;

            long[,] lpv = new long[m_numRows, m_numCols];
            //for (int i = 0; i < m_numValues; i++) lpv[i] = vl[i];

            //Store the double array
            //if (mp_longValues) delete[] mp_longValues;

            //Create the tab-delimited memo string
            //MemoFieldProcessing* mfp = new MemoFieldProcessing();

            //!TODO;rdc critical;19Dec2018
            //char* s = mfp->dataToMemoString(lpv, numRows, numCols);
            //this->setValueMemo(s);

            //if (mfp) delete mfp;
            //mfp = NULL;

            //if (lpv) delete[] lpv;
            lpv = null;

        }

        void setValueMemo(long numRowsIn,
                            double[] x,
                            double[] y1 = null,
                            double[] y2 = null,
                            double[] y3 = null,
                            double[] y4 = null,
                            double[] y5 = null,
                            double[] y6 = null,
                            double[] y7 = null)
        {
            /*
              This stores an array of doubles in the DbfFieldInfo object as a tab-
                delimited character string.
              The number of columns is controlled by the arguments.
              It sets both the double array as well as the conversion to a tab delimited
                memo string.
            */
            double[,] lpv = null;
            double[][] lp = new double[8][];
            int numCols = 0;
            long numRows = numRowsIn;

            if (x != null) numCols++;
            if (y1 != null) numCols++;
            if (y2 != null) numCols++;
            if (y3 != null) numCols++;
            if (y4 != null) numCols++;
            if (y5 != null) numCols++;
            if (y6 != null) numCols++;
            if (y7 != null) numCols++;

            long numValues = numRows * numCols;

            if (numValues > 0)
            {
                m_numRows = numRows;
                m_numCols = numCols;
                m_numValues = numValues;

                lpv = new double[m_numRows, m_numCols];
                double[] pcol = null;

                int mcum = 0;

                lp[0] = x;
                lp[1] = y1;
                lp[2] = y2;
                lp[3] = y3;
                lp[4] = y4;
                lp[5] = y5;
                lp[6] = y6;
                lp[7] = y7;

                for (int icol = 0; icol < numCols; icol++)
                {
                    pcol = lp[icol];
                    for (int irow = 0; irow < numRows; irow++, mcum++)
                    {
                        lpv[irow, icol] = pcol[irow];
                    }
                }

                //Create the tab-delimited memo string
                //MemoFieldProcessing* mfp = new MemoFieldProcessing(); //!TODO;rdc critical;19Dec2018

                //char* s = mfp->dataToMemoString(lpv, numRows, numCols);
                //this->setValueMemo(s);

                //Store the double array
                //if (mp_doubleValues) delete[] mp_doubleValues;
                mp_doubleValues = null;

                //mp_doubleValues = new double[m_numValues];
                //for (int i = 0; i < m_numValues; i++) mp_doubleValues[i] = lpv[i];
            }
            else
            {
                m_numRows = 0;
                m_numCols = 0;
                m_numValues = 0;
            }
            //if (lpv) delete[] lpv;
            lpv = null;
        }

        public void setValueMemoLegacy(double[,] v, long numRows, long numCols)
        {

        }

        /*
        //Set Memo Values when building from multiple fields (eg. old data is
        //  stored in legacy fields and new data is stored in table format).
        //Process:

            1) setNumRowsAndCols (resets the data)
            2) setValueMemoAdd(double* v1, double* v2 = NULL), etc.  or
                setValueMemoAdd(const int numCols, double* v);  (adds multiple columns)
            3) setValueMemoAddFlush Then, close it out and form the memo string.
        //
        */
        void setValueMemoAdd(double[] v0,
                            double[] v1 = null,
                            double[] v2 = null,
                            double[] v3 = null,
                            double[] v4 = null,
                            double[] v5 = null,
                            double[] v6 = null,
                            double[] v7 = null,
                            double[] v8 = null)
        {
            /*
              Must call setNumRowsAndCols first
            */
            int numCols = 0;
            long ixCum = m_ixColumn * m_numRows;
            double[][] lp = new double[9][];
            double[] pcol = null;

            if (v0 != null) numCols++;
            if (v1 != null) numCols++;
            if (v2 != null) numCols++;
            if (v3 != null) numCols++;
            if (v4 != null) numCols++;
            if (v5 != null) numCols++;
            if (v6 != null) numCols++;
            if (v7 != null) numCols++;
            if (v8 != null) numCols++;

            lp[0] = v0;
            lp[1] = v1;
            lp[2] = v2;
            lp[3] = v3;
            lp[4] = v4;
            lp[5] = v5;
            lp[6] = v6;
            lp[7] = v7;
            lp[8] = v8;

            for (int icol = 0; icol < numCols; icol++, m_ixColumn++)
            {
                pcol = lp[icol];

                for (int irow = 0; irow < m_numRows; irow++, ixCum++)
                {
                    //mp_doubleValues[ixCum] = pcol[irow];
                    mp_doubleValues[irow, icol] = pcol[irow];
                }
            }

        }

        void setValueMemoAdd(int numCols, double[] v)
        {
            //!TODO;rdc critical;19Dec2018
        }
        void setValueMemoAddFlush()
        {
            //!TODO;rdc critical;19Dec2018
        }

        //Get methods
        long getNumRows()
        {
            return m_numRows;
        }
        long getNumCols()
        {
            return m_numCols;
        }

        public Object getValue()
        {
            switch (m_dataType)
            {
                //Integer / Long
                case ('N'):
                    return (Object)m_longValue;
                //double / floating point
                case ('F'):
                    //case(r4numDoub):
                    return (Object)m_doubleValue;
                //Logical
                case ('L'):
                    return (Object)m_logicalValue;
                //Memo or String
                case ('B'):
                case ('S'):
                case ('M'):
                    //case (r4memo_binary):
                    return (Object)mp_memoValue;
                //Date
                case ('D'):
                    break;
            }
            return null;

        }
        double[,] getValue(ref long numRows, ref long numCols)
        {
            /*
              Returns an array of doubles that were previously retrieved from the database.
            */

            numRows = m_numRows;
            numCols = m_numCols;

            return mp_doubleValues;
        }

        int getValueInt()
        {
            return (int)m_longValue;
        }
        long getValueLong()
        {
            return m_longValue;
        }
        char getValueLogical()
        {
            return m_logicalValue;
        }
        double getValueDouble()
        {
            return m_doubleValue;
        }
        string getValueMemo()
        {
            return mp_memoValue;
        }

        double[,] getValueMemoData()
        {
            return mp_doubleValues;
        }
        double[,] getValueMemoData(ref double[,] v, ref long numRows, ref long numCols)
        {
            /*
              Returns a pointer to the data in this memory. Fills v with values. It must
              have previously been allocated.
            */
            int m = 0;
            numRows = m_numRows;
            numCols = m_numCols;

            for (int icol = 0; icol < numCols; icol++)
            {
                for (int irow = 0; irow < numRows; irow++)
                {
                    //m = icol * numRows + irow;
                    //v[m] = this->mp_doubleValues[m];
                    v[irow, icol] = mp_doubleValues[irow, icol];
                }
            }
            return mp_doubleValues;
        }
        long[,] getValueMemoData(ref long[,] lv, ref long numRows, ref long numCols)
        {
            /*
              Returns a pointer to the data in this memory. Fills lv with values. It must
              have previously been allocated.
            */
            int m = 0;
            numRows = m_numRows;
            numCols = m_numCols;

            for (int icol = 0; icol < numCols; icol++)
            {
                for (int irow = 0; irow < numRows; irow++)
                {
                    //m = icol * numRows + irow;
                    //lv[m] = this->mp_longValues[m];
                    lv[irow, icol] = mp_longValues[irow, icol];
                }
            }
            return mp_longValues;
        }
        long[,] getValueMemoDataLong(ref long numRows, ref long numCols)
        {
            numRows = m_numRows;
            numCols = m_numCols;
            return mp_longValues;
        }
        double[,] getValueMemoData(int ixCol)
        {
            /*
              Returns a pointer to a specific column of data in memory. ixCol starts at 1.
            */
            //int m = ixCol * this->m_numRows;
            //return &this->mp_doubleValues[m];
            //!TODO;rdc critical;19Dec2018
            return mp_doubleValues;

        }
        double[] getValueMemoData(int ixCol, double[] v)
        {
            /*
              Returns a pointer to a specific column of data in memory. ixCol starts at 1.
              It fills in the array v which must have been previously allocated.
            */
            long m = ixCol * m_numRows;
            for (int irow = 0; irow < m_numRows; irow++)
            {
                //v[irow] = this->mp_doubleValues[m + irow];
                v[irow] = mp_doubleValues[irow, ixCol - 1];
            }
            return v;
        }

        double[,] getValueMemo(ref long numRows, ref long numCols, ref double[] x, ref double[] y)
        {
            double[,] v = null;

            numRows = m_numRows;
            numCols = m_numCols;
            int m = 0;
            double vs;

            if (numRows > 0 && numCols > 0)
            {
                //if (x) delete[] x;
                //if (y) delete[] y;
                x = new double[numRows];
                y = new double[numRows];
                for (int ic = 0; ic < numCols; ic++)
                {
                    for (int ir = 0; ir < numRows; ir++)
                    {
                        //m = ic * numRows + ir;
                        //vs = this->mp_doubleValues[m];
                        vs = mp_doubleValues[ir, ic];
                        switch (ic)
                        {
                            case 0:
                                x[ir] = vs;
                                break;
                            case 1:
                                y[ir] = vs;
                                break;
                            case 2:
                                break;
                            case 3:
                                break;
                            case 4:
                                break;
                            case 5:
                                break;
                            case 6:
                                break;
                        }
                    }
                }
                v = mp_doubleValues;
            }
            else
            {
                numRows = 0;
                numCols = 0;
            }
            return v;
        }

        double[,] getValueMemo(ref long numRows,
                       ref long numCols,
                       double[] x,
                       double[] y1,
                       double[] y2,
                       double[] y3,
                       double[] y4,
                       double[] y5,
                       double[] y6)
        {
            /*
              May want to do this in memoFieldProcessing
            */
            double[,] v = null;

            numRows = m_numRows;
            numCols = m_numCols;
            int m = 0;
            double vs;

            if (numRows > 0 && numCols > 0)
            {
                x = new double[numRows];
                y1 = new double[numRows];
                y2 = new double[numRows];
                y3 = new double[numRows];
                y4 = new double[numRows];
                y5 = new double[numRows];
                y6 = new double[numRows];

                for (int ic = 0; ic < numCols; ic++)
                {
                    for (int ir = 0; ir < numRows; ir++)
                    {
                        //m = ic * numRows + ir;
                        //vs = this->mp_doubleValues[m];
                        vs = mp_doubleValues[ir, ic];

                        switch (ic)
                        {
                            case 0:
                                x[ir] = vs;
                                break;
                            case 1:
                                y1[ir] = vs;
                                break;
                            case 2:
                                y2[ir] = vs;
                                break;
                            case 3:
                                y3[ir] = vs;
                                break;
                            case 4:
                                y4[ir] = vs;
                                break;
                            case 5:
                                y5[ir] = vs;
                                break;
                            case 6:
                                y6[ir] = vs;
                                break;
                        }
                    }
                }
                v = mp_doubleValues;
            }
            else
            {
                numRows = 0;
                numCols = 0;
            }
            return v;

        }

        //Pass data to the data file data methods interacting with data file
        /*
        void*   readValue()
        {

        }
        void    writeValue()
        {

        }
        void    readOneFieldData(ofstream &w)
        {

        }
        FIELD4INFO* fieldInfoToField4Info();
        */
    }
}