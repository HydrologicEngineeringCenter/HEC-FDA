using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;
namespace DbMemory
{
    public class NextIdMgr
    {
        public int ObjectType = 0;
        public long NextObjectID = 1L;
        public long NextObjIdPlan { get; set; }
        public long NextObjIdYear { get; set; }
        public long NextObjIdStream { get; set; }
        public long NextObjIdReach { get; set; }
        public long NextObjIdDmgCat { get; set; }
        public long NextObjIdOccType { get; set; }
        public long NextObjIdModule { get; set; }
        public long NextObjIdStruct { get; set; }
        public long NextObjIdWspData { get; set; }
        public long NextObjIdFreqData { get; set; }
        public long NextObjIdRatData { get; set; }
        public long NextObjIdLevData { get; set; }
        public long NextObjIdAggDfData { get; set; }
        public long NextObjIdEadData { get; set; }

        bool _FlushIt = false;


        //------------------------------------------------------------------------------
        //Methods
        public NextIdMgr()
        {
            this._FlushIt = false;
            setDefault();
        }
        public void setDefault()
        {
            this.setNextObjectIDGlobal(0L);

            this.setNextObjIdPlan();     // 1
            this.setNextObjIdYear();     // 2
            this.setNextObjIdStream();   // 3
            this.setNextObjIdReach();    // 4
            this.setNextObjIdDmgCat();   // 5
            this.setNextObjIdOccType();  // 6
            this.setNextObjIdModule();   // 7
            this.setNextObjIdStruct();   // 8
            this.setNextObjIdWspData();  // 9
            this.setNextObjIdFreqData(); //10
            this.setNextObjIdRatData();  //11
            this.setNextObjIdLevData();  //12
            this.setNextObjIdAggDfData();//13
            this.setNextObjIdEadData();  //14

            _FlushIt = true;
        }
        public void setObjectType(int ixId)
        {
            /*
              Sets the current object type (plan, year, etc.)
                index starts at zero.
            this.m_objectType = ixId;

            if (ixId < 14)
                mp_getNextObjId = mp_getNextObjIdList[ixId];
            else
                mp_getNextObjId = &NextIdMgr::getNextObjectIDGlobal;
            */

        }
        public void setFlushIt(bool flushIt = true)
        {
            _FlushIt = flushIt;
        }
        public bool shouldFlushIt() { return _FlushIt; }

        //Exception readRecord();
        //Exception writeRecord();
        // Exception transferFDObjectToDbf();
        //Exception transferDbfToFDObject();

        public long getNextObjectIDGlobal()
        {
            NextObjectID++;
            return NextObjectID;
        }
        public long decrementObjectIDGlobal(long inc = +1L)
        {
            NextObjectID = NextObjectID - inc;
            return NextObjectID;
        }
        public void setNextObjectIDGlobal(long id = -1L)
        {
            NextObjectID = id;
        }
        public long getNextObjectID() { return NextObjectID; }

        public long getCurrentObjIdPlan() { return NextObjIdPlan; }
        public long getCurrentObjIdYear() { return NextObjIdYear; }
        public long getCurrentObjIdStream() { return NextObjIdStream; }
        public long getCurrentObjIdReach() { return NextObjIdReach; }
        public long getCurrentObjIdDmgCat() { return NextObjIdDmgCat; }
        public long getCurrentObjIdOccType() { return NextObjIdOccType; }
        public long getCurrentObjIdModule() { return NextObjIdModule; }
        public long getCurrentObjIdStruct() { return NextObjIdStruct; }
        public long getCurrentObjIdWspData() { return NextObjIdWspData; }
        public long getCurrentObjIdFreqData() { return NextObjIdFreqData; }
        public long getCurrentObjIdRatData() { return NextObjIdRatData; }
        public long getCurrentObjIdLevData() { return NextObjIdLevData; }
        public long getCurrentObjIdAggDfData() { return NextObjIdAggDfData; }
        public long getCurrentObjIdEadData() { return NextObjIdEadData; }

        public long getNextObjIdPlan() { return ++NextObjIdPlan; }
        public long getNextObjIdYear() { return ++NextObjIdYear; }
        public long getNextObjIdStream() { return ++NextObjIdStream; }
        public long getNextObjIdReach() { return ++NextObjIdReach; }
        public long getNextObjIdDmgCat() { return ++NextObjIdDmgCat; }
        public long getNextObjIdOccType() { return ++NextObjIdOccType; }
        public long getNextObjIdModule() { return ++NextObjIdModule; }
        public long getNextObjIdStruct() { return ++NextObjIdStruct; }
        public long getNextObjIdWspData() { return ++NextObjIdWspData; }
        public long getNextObjIdFreqData() { return ++NextObjIdFreqData; }
        public long getNextObjIdRatData() { return ++NextObjIdRatData; }
        public long getNextObjIdLevData() { return ++NextObjIdLevData; }
        public long getNextObjIdAggDfData() { return ++NextObjIdAggDfData; }
        public long getNextObjIdEadData() { return ++NextObjIdEadData; }

        public long setNextObjIdPlan(long id = +1L) { NextObjIdPlan = id; return NextObjIdPlan; }
        public long setNextObjIdYear(long id = +1L) { NextObjIdYear = id; return NextObjIdYear; }
        public long setNextObjIdStream(long id = +1L) { NextObjIdStream = id; return NextObjIdStream; }
        public long setNextObjIdReach(long id = +1L) { NextObjIdReach = id; return NextObjIdReach; }
        public long setNextObjIdDmgCat(long id = +1L) { NextObjIdDmgCat = id; return NextObjIdDmgCat; }
        public long setNextObjIdOccType(long id = +1L) { NextObjIdOccType = id; return NextObjIdOccType; }
        public long setNextObjIdModule(long id = +1L) { NextObjIdModule = id; return NextObjIdModule; }
        public long setNextObjIdStruct(long id = +1L) { NextObjIdStruct = id; return NextObjIdStruct; }
        public long setNextObjIdWspData(long id = +1L) { NextObjIdWspData = id; return NextObjIdWspData; }
        public long setNextObjIdFreqData(long id = +1L) { NextObjIdFreqData = id; return NextObjIdFreqData; }
        public long setNextObjIdRatData(long id = +1L) { NextObjIdRatData = id; return NextObjIdRatData; }
        public long setNextObjIdLevData(long id = +1L) { NextObjIdLevData = id; return NextObjIdLevData; }
        public long setNextObjIdAggDfData(long id = +1L) { NextObjIdAggDfData = id; return NextObjIdAggDfData; }
        public long setNextObjIdEadData(long id = +1L) { NextObjIdEadData = id; return NextObjIdEadData; }

        public long decrementObjIdPlan(long inc = +1L) { NextObjIdPlan = NextObjIdPlan - inc; return NextObjIdPlan; }
        public long decrementObjIdYear(long inc = +1L) { NextObjIdYear = NextObjIdYear - inc; return NextObjIdYear; }
        public long decrementObjIdStream(long inc = +1L) { NextObjIdStream = NextObjIdStream - inc; return NextObjIdStream; }
        public long decrementObjIdReach(long inc = +1L) { NextObjIdReach = NextObjIdReach - inc; return NextObjIdReach; }
        public long decrementObjIdDmgCat(long inc = +1L) { NextObjIdDmgCat = NextObjIdDmgCat - inc; return NextObjIdDmgCat; }
        public long decrementObjIdOccType(long inc = +1L) { NextObjIdOccType = NextObjIdOccType - inc; return NextObjIdOccType; }
        public long decrementObjIdModule(long inc = +1L) { NextObjIdModule = NextObjIdModule - inc; return NextObjIdModule; }
        public long decrementObjIdStruct(long inc = +1L) { NextObjIdStruct = NextObjIdStruct - inc; return NextObjIdStruct; }
        public long decrementObjIdWspData(long inc = +1L) { NextObjIdWspData = NextObjIdWspData - inc; return NextObjIdWspData; }
        public long decrementObjIdFreqData(long inc = +1L) { NextObjIdFreqData = NextObjIdFreqData - inc; return NextObjIdFreqData; }
        public long decrementObjIdRatData(long inc = +1L) { NextObjIdRatData = NextObjIdRatData - inc; return NextObjIdRatData; }
        public long decrementObjIdLevData(long inc = +1L) { NextObjIdLevData = NextObjIdLevData - inc; return NextObjIdLevData; }
        public long decrementObjIdAggDfData(long inc = +1L) { NextObjIdAggDfData = NextObjIdAggDfData - inc; return NextObjIdAggDfData; }
        public long decrementObjIdEadData(long inc = +1L) { NextObjIdEadData = NextObjIdEadData - inc; return NextObjIdEadData; }
        public void Print()
        {
            WriteLine("\n\nNext ID data");
            WriteLine($"\tPlan ID: {NextObjIdPlan}");
            WriteLine($"\tYear ID: {NextObjIdYear}");
            WriteLine($"\tStream ID: {NextObjIdStream}");
            WriteLine($"\tReach ID: {NextObjIdReach}");
            WriteLine($"\tCategory ID: {NextObjIdDmgCat}");
            WriteLine($"\tOccupancy Type ID: {NextObjIdOccType}");
            WriteLine($"\tStructure Module ID: {NextObjIdModule}");
            WriteLine($"\tStructure ID: {NextObjIdStruct}");
            WriteLine($"\tWater Surface Profile ID: {NextObjIdWspData}");
            WriteLine($"\tProbability Function ID: {NextObjIdFreqData}");
            WriteLine($"\tRating Curve ID: {NextObjIdRatData}");
            WriteLine($"\tLevee Data ID: {NextObjIdLevData}");
            WriteLine($"\tAggregated Stage-Damage ID: {NextObjIdAggDfData}");
            WriteLine($"\tExpected Annual Damnage ID: {NextObjIdEadData}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine("\n\nNext ID data");
            wr.WriteLine($"\tPlan ID: {NextObjIdPlan}");
            wr.WriteLine($"\tYear ID: {NextObjIdYear}");
            wr.WriteLine($"\tStream ID: {NextObjIdStream}");
            wr.WriteLine($"\tReach ID: {NextObjIdReach}");
            wr.WriteLine($"\tCategory ID: {NextObjIdDmgCat}");
            wr.WriteLine($"\tOccupancy Type ID: {NextObjIdOccType}");
            wr.WriteLine($"\tStructure Module ID: {NextObjIdModule}");
            wr.WriteLine($"\tStructure ID: {NextObjIdStruct}");
            wr.WriteLine($"\tWater Surface Profile ID: {NextObjIdWspData}");
            wr.WriteLine($"\tProbability Function ID: {NextObjIdFreqData}");
            wr.WriteLine($"\tRating Curve ID: {NextObjIdRatData}");
            wr.WriteLine($"\tLevee Data ID: {NextObjIdLevData}");
            wr.WriteLine($"\tAggregated Stage-Damage ID: {NextObjIdAggDfData}");
            wr.WriteLine($"\tExpected Annual Damnage ID: {NextObjIdEadData}");
        }

    }
}
