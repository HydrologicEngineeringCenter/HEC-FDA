using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using static System.Console;

namespace DbMemory
{
    [Serializable]
    public class EadResult : FdObjectDataLook
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2018
        #endregion
        #region Fields
        #endregion
        #region Properties
        public static int MAX_ORDS_PROB_DAMG = 200;
        public static int CATEGORY_SIZE = 20;
        public int Iunc { get; set; }
        public double Ptarg { get; set; }
        public double Fdmtrg { get; set; }
        public int Nyears { get; set; }
        public double Drate { get; set; }

        public int Istics { get; set; }
        public int Nstics { get; set; }
        public int NumRows_SimStatTable { get; set; }
        public double[] Dmean { get; set; }
        public double[] Dsdev { get; set; }
        public double[] Dskew { get; set; }


        public int NumRows_PPNONEXTBL { get; set; }
        public int NumCols_PPNONEXTBL { get; set; }

        public int Ncalc { get; set; }
        public double[] Pcalc { get; set; }
        public int Nevstp { get; set; }
        public double[] PcalcNonExceed { get; set; }
        public double[] Sevent { get; set; }
        public int[] Ntable { get; set; }

        public int Nfr { get; set; }
        public double[] Prob { get; set; }
        public double[] Qprob { get; set; }
        public double[] Qtprob { get; set; }
        public double[] Stprob { get; set; }
        public int Ncat { get; set; }
        public double[] Dprob { get; set; }

        public double Starg { get; set; }
        public double Pargmn { get; set; }
        public double Pargmd { get; set; }

        public int Nevent { get; set; }
        public int NumRows_Ppnonextar { get; set; }
        public int NumCols_Ppnonextar { get; set; }
        public double[] Pevent { get; set; }
        public double[] Targev { get; set; }

        public int Nrisk { get; set; }
        public double[] Targyr { get; set; }
        public double[] Targrk { get; set; }

        public int Kcat { get; set; }
        public int NumRows_EadCum_Tbl { get; set; }
        public int NumRows_SIMEAD_TBL { get; set; }
        public int[] IdCat { get; set; }
        public double[] IdCatD { get; set; }
        public double[] Ead { get; set; }
        public double[] Cead { get; set; }

        public int NcatWoUncert { get; set; }
        public int[] IdCatWoUncert { get; set; }
        public double[] IdCatWoUncertD { get; set; }
        public double[] EadWoUncert { get; set; }
        public int NfrWoUncert { get; set; }
        public double[] ProbWoUncert { get; set; }
        public double[] FlowWoUncert { get; set; }
        public double[] StageWoUncert { get; set; }
        public double[] DamageWoUncert { get; set; }

        public int NetBl { get; set; }
        public double[] Ptead { get; set; }
        public double[] Ctead { get; set; }

        public int KclassEad { get; set; }
        public int KclassAep { get; set; }
        public int NumSimulations { get; set; }
        public int NumClassEad { get; set; }
        public int NumClassAep { get; set; }
        public int[] NhitEad { get; set; }
        public int[] NhitAep { get; set; }
        public double[] EadClass { get; set; }
        public double[] EadFreq { get; set; }
        public double[] AepClass { get; set; }
        public double[] AepFreq { get; set; }
        public double[] EadFreqI { get; set; }
        public double[] AepFreqI { get; set; }

        public int KclassStd { get; set; }
        public int NclassStd { get; set; }
        public double[] AepStandard { get; set; }
        public double[] FreqStandard { get; set; }
        public double[] FreqStandardI { get; set; }

        string VerDateMeth { get; set; }
        public int US { get; private set; }

        #endregion
        #region Constructors
        public EadResult() : base()
        {
            CalculationDate = "";
            IsValid = false;
            IsOutOfDate = true;
            Kcat = CATEGORY_SIZE;

            KclassEad = 0;
            KclassAep = 0;
            NumSimulations = 0;
            NumClassEad = 0;
            NumClassAep = 0;
            KclassStd = 0;
            NclassStd = 0;

            Dmean = null;
            Dsdev = null;
            Dskew = null;
            Pcalc = null;
            PcalcNonExceed = null;
            Ntable = null;
            Prob = null;
            Qprob = null;
            Qtprob = null;
            Stprob = null;
            Dprob = null;
            Pevent = null;
            Sevent = null;
            Targyr = null;
            Targrk = null;
            Targev = null;
            IdCat = new int[Kcat];
            IdCatD = new double[Kcat];
            Ead = null;
            Cead = null;
            Ptead = null;
            Ctead = null;
            NhitEad = null;
            NhitAep = null;
            EadClass = null;
            EadFreq = null;
            EadFreqI = null;
            AepFreqI = null;
            AepStandard = null;
            FreqStandard = null;
            FreqStandardI = null;

            VerDateMeth = "";

            NcatWoUncert = 0;
            NfrWoUncert = 0;
            IdCatWoUncert = new int[Kcat];
            IdCatWoUncertD = new double[Kcat];
            EadWoUncert = new double[Kcat];
            ProbWoUncert = new double[MAX_ORDS_PROB_DAMG];
            FlowWoUncert = new double[MAX_ORDS_PROB_DAMG];
            StageWoUncert = new double[MAX_ORDS_PROB_DAMG];
            DamageWoUncert = new double[MAX_ORDS_PROB_DAMG * CATEGORY_SIZE];

            reset();
        }
        #endregion
        #region Voids
        public void SetDate(string date)    // This sets the results valid.  It is the only way to do so.
        {
            CalculationDate = date;
            IsValid = true;
        }
        public void markExistingResultsInvalid()
        {
            IsValid = false;
        }

        #endregion
        #region Functions
        public void deallocate()
        {
            ;
        }
        public new void reset()
        {
            base.reset();

            CalculationDate = "";
            IsValid = false;
            IsOutOfDate = true;

            Iunc = 1;
            Ptarg = 0.01;
            Fdmtrg = 5.0;
            Nyears = 50;
            Drate = 4.0;

            Istics = 0;
            Nstics = 0;
            //Dmean, Dsdev, Dskew

            Ncalc = 0;
            //Pcalc, PcalcNonExceed, Ntable

            Nfr = 0;
            //Prob, Qprob, Qtprob, stprob
            Ncat = 0;
            // Dprob

            Starg = 0;
            Pargmn = 0;
            Pargmd = 0;
            Nevent = 0;
            //Pevent
            Nevstp = 0;
            //Sevent;
            Nrisk = 0;
            //Targyr, Targrk, Targev

            NetBl = 0;
            //Ead, Cead, Ctead

            KclassEad = 0;
            KclassAep = 0;
            NumSimulations = 0;
            NumClassEad = 0;
            NumClassAep = 0;

            NclassStd = 0;
            NfrWoUncert = 0;
        }
        public void ResetAndDeallocate()
        {
            reset();
            deallocate();
        }
        public string SetVersionDateMethod(string theVerDateMeth)
        {
            VerDateMeth = theVerDateMeth;
            return VerDateMeth;
        }
        public string SetVersionDateMethod()
        {
            VerDateMeth = GlobalVariables.mp_fdaStudy.DetermineVersionDate();
            return VerDateMeth;
        }
        public string getVersionDateMethod() { return VerDateMeth; }


        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // set "expected" values
        public void setAnalysisParameters(int iunc, double ptarg, double fdmtrg)
        {
            Iunc = iunc;
            Ptarg = ptarg;
            Fdmtrg = fdmtrg;
        }


        //Output Statistics
        public void SetOutputStatistics(int istics,
                                          int nstics,
                                          double[] dmean,
                                          double[] dsdev,
                                          double[] dskew)
        {
            Istics = istics;
            Nstics = nstics;

            Dmean = new double[Nstics];
            Dsdev = new double[Nstics];
            Dskew = new double[Nstics];

            for (int i = 0; i < Nstics; i++)
            {
                Dmean[i] = dmean[i];
                Dsdev[i] = dsdev[i];
                Dskew[i] = dskew[i];
            }
        }
        public void SetComputationalRecord(int istics,    //deprecated
                                          int nstics,
                                          double[] dmean,
                                          double[] dsdev,
                                          double[] dskew)
        {
            SetOutputStatistics(istics, nstics, dmean, dsdev, dskew);
        }

        public void SetComputationalRecord(int istics,    //deprecated
                                          int nstics,
                                          float[] dmean,
                                          float[] dsdev,
                                          float[] dskew)
        {
            Istics = istics;
            Nstics = nstics;

            Dmean = new double[Nstics];
            Dsdev = new double[Nstics];
            Dskew = new double[Nstics];

            for (int i = 0; i < Nstics; i++)
            {
                Dmean[i] = (double)dmean[i];
                Dsdev[i] = (double)dsdev[i];
                Dskew[i] = (double)dskew[i];
            }
        }

        //Probability Array and Referencing
        public void SetArrayIndexing(int ncalc, double[] pcalc, int[] ntable)
        {
            Ncalc = ncalc;

            Pcalc = new double[Ncalc];
            PcalcNonExceed = new double[Ncalc];
            //Pevent = new double[Ncalc];
            Ntable = new int[Ncalc];

            for (int i = 0; i < Ncalc; i++)
            {
                Pcalc[i] = pcalc[i];
                PcalcNonExceed[i] = 1.0 - Pcalc[i];
                //Pevent[i] = PcalcNonExceeed[i];
                Ntable[i] = ntable[i];
            }
        }
        public void SetArrayIndexing(int ncalc, float[] pcalc, int[] ntable)
        {
            Ncalc = ncalc;

            Pcalc = new double[Ncalc];
            PcalcNonExceed = new double[Ncalc];
            //Pevent = new double[Ncalc];
            Ntable = new int[Ncalc];

            for (int i = 0; i < Ncalc; i++)
            {
                Pcalc[i] = (double)pcalc[i];
                PcalcNonExceed[i] = 1.0 - Pcalc[i];
                //Pevent[i] = PcalcNonExceeed[i];
                Ntable[i] = ntable[i];
            }
        }
        //Average Curves
        public void SetOutputAvgCurves(int nfr,
                                     float[] prob,
                                     float[] qprob,
                                     float[] qtprob,
                                     float[] stprob,
                                     int ncat,
                                     float[] dprob)
        {
            int numVals = 0;
            int i = 0;

            if (nfr > 0)
            {
                double[] pr = new double[nfr];
                double[] qp = new double[nfr];
                double[] qt = new double[nfr];
                double[] st = new double[nfr];
                //double[] dp = new double[nfr];
                numVals = nfr * (ncat + 1);
                double[] dp = new double[numVals];

                for (i = 0; i < nfr; i++)
                {
                    pr[i] = (double)prob[i];
                    qp[i] = (double)qprob[i];
                    qt[i] = (double)qtprob[i];
                    st[i] = (double)stprob[i];
                }
                for (i = 0; i < numVals; i++)
                {
                    dp[i] = dprob[i];
                }
                SetOutputAvgCurves(nfr, pr, qp, qt, st, ncat, dp);
            }
        }
        //Average Curves
        //rdc hail Mary;08Jul2010
        public void SetOutputAvgCurves(int nfr,
                                     double[] prob,
                                     double[] qprob,
                                     double[] qtprob,
                                     double[] stprob,
                                     int ncat,
                                     double[] dprob)
        {
            int numVals = 0;
            int i = 0, j = 0, m = 0, ix = 0, mt = 0;
            double v = 0.0;

            Nfr = nfr;

            Prob = new double[Nfr];
            Qprob = new double[Nfr];
            Qtprob = new double[Nfr];
            Stprob = new double[Nfr];

            for (i = 0; i < Nfr; i++)
            {
                Prob[i] = prob[i];
                Qprob[i] = qprob[i];
                Qtprob[i] = qtprob[i];
                Stprob[i] = stprob[i];
            }

            numVals = Nfr * (ncat + 1);
            Dprob = new double[numVals];

            m = Nfr * (ncat + 1);
            mt = Nfr * ncat;
            for (i = 0; i < m; i++) Dprob[i] = 0.0;

            for (j = 0; j < ncat; j++)
            {
                for (i = 0; i < nfr; i++)
                {
                    ix = j * Nfr + i;
                    v = dprob[ix];
                    Dprob[ix] = v;
                    Dprob[mt + i] += v; //compute total damage
                }
            }
        }
        public void SetOutputResultArrays(int nfr,        //deprecated
                                         double[] prob,
                                         double[] qprob,
                                         double[] qtprob,
                                         double[] stprob,
                                         int ncat,
                                         double[] dprob)
        {
            SetOutputAvgCurves(nfr, prob, qprob, qtprob, stprob, ncat, dprob);
        }
        public void SetOutputResultArrays(int nfr,        //deprecated
                                         float[] prob,
                                         float[] qprob,
                                         float[] qtprob,
                                         float[] stprob,
                                         int ncat,
                                         float[] dprob)
        {
            int numVals = 0;
            int i = 0, j = 0, m = 0, ix = 0, mt = 0;
            double v = 0.0;

            Nfr = nfr;

            Prob = new double[Nfr];
            Qprob = new double[Nfr];
            Qtprob = new double[Nfr];
            Stprob = new double[Nfr];

            for (i = 0; i < Nfr; i++)
            {
                Prob[i] = (double)prob[i];
                Qprob[i] = (double)qprob[i];
                Qtprob[i] = (double)qtprob[i];
                Stprob[i] = (double)stprob[i];
            }
            numVals = Nfr * (ncat + 1);
            Dprob = new double[numVals];

            m = Nfr * (ncat + 1);
            mt = Nfr * ncat;
            for (i = 0; i < m; i++) Dprob[i] = 0.0;

            for (j = 0; j < ncat; j++)
            {
                for (i = 0; i < Nfr; i++)
                {
                    ix = j * Nfr + i;
                    v = Dprob[ix];
                    Dprob[ix] = v;
                    Dprob[mt + i] += v;
                }
            }
        }
        //AEP and target Stage
        public void SetTargetAep(double starg, double pargmn, double pargmd)
        {
            Starg = starg;
            Pargmn = pargmn;
            Pargmd = pargmd;
        }
        public void SetTargetStageData(double starg, double pargmn, double pargmd)
        {
            SetTargetAep(starg, pargmn, pargmd);
        }
        //Conditional Non-Exceedance Data
        //Exception setOutputConditionalNonExceedance(long nevent,
        //                                            double* pevent,
        //                                            long    nevstp,
        //                                            double* sevent,
        //                                            double* targev);
        public void SetOutputConditionalNonExceedanceTarget(int nevent,
                                                          double[] pevent,
                                                          double[] targev)
        {
            Nevent = nevent;
            Pevent = new double[Nevent];
            Targev = new double[Nevent];


            for (int i = 0; i < Nevent; i++)
            {
                Pevent[i] = pevent[i];
                Targev[i] = targev[i];
            }
        }

        public void SetOutputConditionalNonExceedance(int nevent,
                                                    int nevstp,
                                                    double[] pcalcNonExceed,
                                                    double[] sevent)
        {
            Nevstp = nevstp;
            PcalcNonExceed = new double[nevstp];
            Nevent = nevent;
            Sevent = new double[(Nevent) * Nevstp];

            for (int i = 0; i < Nevstp; i++) PcalcNonExceed[i] = pcalcNonExceed[i];

            int m = 0;
            for (int iev = 0; iev < Nevent; iev++)
            {
                for (int istg = 0; istg < Nevstp; istg++, m++)
                {
                    Sevent[m] = sevent[m];
                }
            }
        }
        //Risk Analysis Data
        public void SetOutputLongTermRisk(int nrisk,
                                        double[] targyr,
                                        double[] targrk)
        {
            int i = 0;
            Nrisk = nrisk;
            Targyr = new double[Nrisk];
            Targrk = new double[Nrisk];

            for (i = 0; i < Nrisk; i++)
            {
                Targyr[i] = targyr[i];
                Targrk[i] = targrk[i];
            }
        }

        //EAD
        public void SetEad(int ncat, int[] idCat, double[] ead)
        {
            int i = 0;
            Ncat = ncat;

            Kcat = Ncat + 1;
            Ead = new double[Kcat];
            IdCat = new int[Kcat];

            Ead[Ncat] = 0.0;
            IdCat[Ncat] = -1;

            for (i = 0; i < Ncat; i++)
            {
                IdCat[i] = idCat[i];
                Ead[i] = ead[i];
                Ead[Ncat] += Ead[i];
            }
        }
        public void SetEadDistrib(int ncalc, double[] pcalc, int[] ntable, double[] cead)
        {
            int i = 0;
            Ncalc = ncalc;

            Pcalc = new double[Ncalc];
            Ntable = new int[Ncalc];
            Cead = new double[Ncalc];

            for (i = 0; i < Ncalc; i++)
            {
                Pcalc[i] = pcalc[i];
                Ntable[i] = ntable[i];
                Cead[i] = cead[i];
            }
        }
        public void SetEadDistribTable(int netbl, double[] ptead, double[] ctead)
        {
            int i = 0;
            NetBl = netbl;
            Ptead = new double[NetBl];
            Ctead = new double[NetBl];

            for (i = 0; i < NetBl; i++)
            {
                Ptead[i] = ptead[i];
                Ctead[i] = ctead[i];
            }
        }
        public void SetEadDistExTable(int numSimulations,
                                    int numClass,
                                    int[] nhitEad,
                                    float[] eadClass,
                                    float[] eadFreq)
        {
            NumSimulations = numSimulations;
            NumClassEad = numClass;

            //Reallocate
            if (KclassEad < NumClassEad)
            {
                NhitEad = new int[KclassEad];
                EadClass = new double[KclassEad];
                EadFreq = new double[KclassEad];
                EadFreqI = new double[KclassEad];
            }
            if (NumClassEad > 0)
            {
                for (int ic = 0; ic < NumClassEad; ic++)
                {
                    NhitEad[ic] = nhitEad[ic];
                    EadClass[ic] = (double)eadClass[ic];
                    EadFreq[ic] = (double)eadFreq[ic];
                    EadFreqI[ic] = (double)(1.0 - EadFreq[ic]);
                }
            }
        }
        public void SetEadDistExTable(int numSimulations,
                                    int numClass,
                                    int[] nhitEad,
                                    double[] eadClass,
                                    double[] eadFreq)
        {
            NumSimulations = numSimulations;
            NumClassEad = numClass;

            if (KclassEad < NumClassEad)
            {
                KclassEad = NumClassEad + 1;
                NhitEad = new int[KclassEad];
                EadClass = new double[KclassEad];
                EadFreq = new double[KclassEad];
                EadFreqI = new double[KclassEad];
            }
            if (NumClassEad > 0)
            {
                for (int ic = 0; ic < NumClassEad; ic++)
                {
                    NhitEad[ic] = nhitEad[ic];
                    EadClass[ic] = eadClass[ic];
                    EadFreq[ic] = eadFreq[ic];
                    EadFreqI[ic] = 1.0 - EadFreq[ic];
                }
            }
        }
        //AEP
        public void SetAepDistExTable(int numSimulations,
                                    int numClass,
                                    int[] nhitAep,
                                    float[] aepClass,
                                    float[] aepFreq)
        {
            NumSimulations = numSimulations;
            NumClassAep = numClass;

            if (KclassAep < NumClassAep)
            {
                KclassAep = NumClassAep + 1;
                NhitAep = new int[KclassAep];
                AepClass = new double[KclassAep];
                AepFreq = new double[KclassAep];
                AepFreqI = new double[KclassAep];
            }
            if (NumClassAep > 0)
            {
                for (int ic = 0; ic < NumClassAep; ic++)
                {
                    NhitAep[ic] = nhitAep[ic];
                    AepClass[ic] = aepClass[ic];
                    AepFreq[ic] = aepFreq[ic];
                    AepFreqI[ic] = 1.0 - (double)AepFreq[ic];
                }
            }
        }
        public void SetAepDistExTable(int numSimulations,
                                    int numClass,
                                    int[] nhitAep,
                                    double[] aepClass,
                                    double[] aepFreq)
        {
            NumSimulations = numSimulations;
            NumClassAep = numClass;

            if (KclassAep < NumClassAep)
            {
                KclassAep = NumClassAep + 1;
                NhitAep = new int[KclassAep];
                AepClass = new double[KclassAep];
                AepFreq = new double[KclassAep];
                AepFreqI = new double[KclassAep];
            }
            if (NumClassAep > 0)
            {
                for (int ic = 0; ic < NumClassAep; ic++)
                {
                    NhitAep[ic] = nhitAep[ic];
                    AepClass[ic] = aepClass[ic];
                    AepFreq[ic] = aepFreq[ic];
                    AepFreqI[ic] = 1.0 - AepFreq[ic];
                }
            }
        }
        public void SetAepDistStdTable(int nclassStd, float[] aepStandard, float[] freqStandard)
        {
            NclassStd = nclassStd;

            if (KclassStd < NclassStd)
            {
                KclassStd = NclassStd;
                AepStandard = new double[KclassStd];
                FreqStandard = new double[KclassStd];
                FreqStandardI = new double[KclassStd];
            }
            for (int i = 0; i < NclassStd; i++)
            {
                AepStandard[i] = aepStandard[i];
                FreqStandard[i] = freqStandard[i];
                FreqStandardI[i] = 1.0 - FreqStandard[i];
            }
        }
        public void SetAepDistStdTable(int nclassStd, double[] aepStandard, double[] freqStandard)
        {
            NclassStd = nclassStd;

            if (KclassStd < NclassStd)
            {
                KclassStd = NclassStd;
                AepStandard = new double[KclassStd];
                FreqStandard = new double[KclassStd];
                FreqStandardI = new double[KclassStd];
            }
            for (int i = 0; i < NclassStd; i++)
            {
                AepStandard[i] = aepStandard[i];
                FreqStandard[i] = freqStandard[i];
                FreqStandardI[i] = 1.0 - FreqStandard[i];
            }
        }


        //Extended Distribution tables
        public void SetDistributionResults(int kclassEad,
                                          int kclassAep,
                                          int numSimulations,
                                          int numClassEad,
                                          int numClassAep,
                                          int[] nhitEad,
                                          int[] nhitAep,
                                          float[] eadClass,
                                          float[] eadFreq,
                                          float[] aepClass,
                                          float[] aepFreq)
        {
            /*
              Store the distribution results from the EAD calculations (both EAD and AEP)
            */
            SetEadDistExTable(numSimulations,
                               numClassEad,
                               nhitEad,
                               eadClass,
                               eadFreq);
            SetAepDistExTable(numSimulations,
                              numClassAep,
                              nhitAep,
                              aepClass,
                              aepFreq);
        }

        //Deprecated Functions
        //Risk Analysis Data
        //rdc hail Mary;08Jul2010
        public void SetRiskAnalysisData(int nevent,
                                       double[] pevent,
                                       int nevstp,
                                       double[] sevent,
                                       int nrisk,
                                       double[] targyr,
                                       double[] targrk,
                                       double[] targev)
        {
            int i = 0;
            Nevent = nevent;
            Pevent = new double[nevent];
            Targev = new double[nevent];

            for (i = 0; i < nevent; i++)
            {
                Pevent[i] = pevent[i];
                Targev[i] = targev[i];
            }
            //  At this point, if no risk, then nevstp is zero.
            //  It tries to allocate sevent with size of zero.
            //  Changed 28Dec2010

            Nevstp = nevstp;
            if (nevstp > 0)
            {
                Sevent = new double[Nevstp];
                for (i = 0; i < Nevstp; i++) Sevent[i] = sevent[i];
            }
            Nrisk = nrisk;
            Targyr = new double[Nrisk];
            Targrk = new double[Nrisk];
            for (i = 0; i < nrisk; i++)
            {
                Targyr[i] = targyr[i];
                Targrk[i] = targrk[i];
            }
        }
        public void SetRiskAnalysisData(int nevent,
                                       float[] pevent,
                                       int nevstp,
                                       float[] sevent,
                                       int nrisk,
                                       float[] targyr,
                                       float[] targrk,
                                       float[] targev)
        {
            double[] targyrD = null;
            double[] targrkD = null;
            double[] peventD = null;
            double[] targevD = null;
            double[] seventD = null;

            if (nrisk > 0)
            {
                targyrD = new double[nrisk];
                targrkD = new double[nrisk];
                for (int i = 0; i < nrisk; i++)
                {
                    targyrD[i] = targyr[i];
                    targrkD[i] = targrk[i];
                }
            }
            if (nevent > 0)
            {
                peventD = new double[nevent];
                targevD = new double[nevent];
                for (int i = 0; i < nevent; i++)
                {
                    peventD[i] = pevent[i];
                    targevD[i] = targevD[i];
                }
            }
            if (nevstp > 0)
            {
                seventD = new double[nevstp];
                for (int i = 0; i < nevstp; i++) seventD[i] = sevent[i];
            }
            SetRiskAnalysisData(nevent, peventD, nevstp, seventD, nrisk, targyrD, targrkD, targevD);
        }


        //rdc hail Mary;08Jul2010

        public void SetDamageData(int nstics,
                                 int ncat,
                                 int[] idCat,
                                 float[] ead,
                                 int ncalc,
                                 float[] cead,
                                 int netbl,
                                 float[] ptead,
                                 float[] ctead)
        {
            int i = 0;

            if (ncat > 0 && ncalc > 0 && netbl > 0)
            {
                double[] eadD = new double[ncat + 1];
                double[] ceadD = new double[ncalc];
                double[] pteadD = new double[netbl];
                double[] cteadD = new double[netbl];

                for (i = 0; i < ncat + 1; i++) eadD[i] = ead[i];

                if (nstics > 0)
                    for (i = 0; i < ncalc; i++) ceadD[i] = cead[i];
                else
                    for (i = 0; i < ncalc; i++) ceadD[i] = 0.0;

                for (i = 0; i < netbl; i++)
                {
                    pteadD[i] = ptead[i];
                    cteadD[i] = ctead[i];
                }
                SetDamageData(ncat, idCat, eadD, ncalc, ceadD, netbl, pteadD, cteadD);
            }
        }


        public void SetDamageData(int ncat,
                                 int[] idCat,
                                 double[] ead,
                                 int ncalc,
                                 double[] cead,
                                 int netbl,
                                 double[] ptead,
                                 double[] ctead)
        {
            int i = 0;

            Ncat = ncat;
            Ncalc = ncalc;
            NetBl = netbl;

            Kcat = Ncat + 1;
            IdCat = new int[Kcat];
            Ead = new double[Kcat];
            Ead[Ncat] = 0.0;
            idCat[Ncat] = -1;
            for (i = 0; i < Ncat; i++)
            {
                IdCat[i] = idCat[i];
                Ead[i] = ead[i];
                ead[Ncat] += ead[i];
            }
            Cead = new double[Ncalc];
            for (i = 0; i < Ncalc; i++) Cead[i] = cead[i];

            Ptead = new double[NetBl];
            Ctead = new double[NetBl];
            for (i = 0; i < NetBl; i++)
            {
                Ptead[i] = ptead[i];
                Ctead[i] = ctead[i];
            }
        }
        void SetCategoryIds(int ncat, int[] idCat)
        {
            if (Kcat < ncat + 1)
            {
                Kcat = ncat + 1;
                IdCat = new int[Kcat];
            }
            Ncat = ncat;
            for (int i = 0; i < Ncat; i++) IdCat[i] = idCat[i];
            IdCat[Ncat] = -1;
        }

        //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // get values
        public void GetDate(ref string date, ref bool validity)
        {
            date = CalculationDate;
            validity = IsValid;
        }

        // When this function is called, date should be an array of size DATE_SIZE,
        //   allocated by the caller.
        // To retrieve validity by itself, call the isNotValid function.

        //----------------------------------------------------------------------------
        // For all of the following functions:
        // -- For all arguments of type []&, pass empty pointers - the function will
        //      allocate memory into them.
        // -- They should not point to allocated memory - the functions WILL NOT call
        //      delete for these pointers before reallocating them.  The functions
        //      will allocate memory and fill it with data.  It is the caller's
        //      responsibility to delete the memory.
        //----------------------------------------------------------------------------
        public void GetAnalysisParameters(ref int iunc, ref double ptarg, ref double fdmtrg)
        {
            iunc = Iunc;
            ptarg = Ptarg;
            fdmtrg = Fdmtrg;
        }
        public int UsedUncertainty() { return Iunc; }

        //Output Statistics
        public void GetOutputStatistics(ref int istics,
                                      ref int nstics,
                                      ref int ncat,
                                      ref double[] dmean,
                                      ref double[] dsdev,
                                      ref double[] dskew)
        {
            istics = Istics;
            nstics = Nstics;
            ncat = Ncat;
            dmean = new double[nstics];
            dsdev = new double[nstics];
            dskew = new double[nstics];

            for (int i = 0; i < nstics; i++)
            {
                dmean[i] = Dmean[i];
                dsdev[i] = Dsdev[i];
                dskew[i] = Dskew[i];
            }
        }
        public void GetComputationalRecord(ref int istics,
                                          ref int nstics,
                                          ref int ncat,
                                          ref double[] dmean,
                                          ref double[] dsdev,
                                          ref double[] dskew)
        {
            GetOutputStatistics(ref istics, ref nstics, ref ncat, ref dmean, ref dsdev, ref dskew);
        }
        //Probability Array and Referencing
        public void GetArrayIndexing(ref int ncalc, ref double[] pcalc, ref int[] ntable)
        {
            ncalc = Ncalc;
            pcalc = new double[Ncalc];
            ntable = new int[Ncalc];

            for (int i = 0; i < Ncalc; i++)
            {
                pcalc[i] = Pcalc[i];
                ntable[i] = Ntable[i];
            }
        }
        //Average Curves
        public void GetOutputAvgCurves(ref int nfr,
                                     ref double[] prob,
                                     ref double[] qprob,
                                     ref double[] qtprob,
                                     ref double[] stprob,
                                     ref int ncat,
                                     ref double[] dprob)
        {
            int i = 0;
            nfr = Nfr;
            ncat = Ncat;

            if (Nfr > 0 && Ncat > 0)
            {
                prob = new double[Nfr];
                qprob = new double[Nfr];
                qtprob = new double[Nfr];
                stprob = new double[Nfr];

                for (i = 0; i < Nfr; i++)
                {
                    prob[i] = Prob[i];
                    qprob[i] = Qprob[i];
                    qtprob[i] = Qtprob[i];
                    stprob[i] = Stprob[i];
                }
                dprob = new double[Nfr * (Ncat + 1)];
                for (i = 0; i < Nfr * (Ncat + 1); i++) dprob[i] = Dprob[i];
            }
        }
        public void GetOutputResultArrays(ref int nfr,
                                         ref double[] prob,
                                         ref double[] qprob,
                                         ref double[] qtprob,
                                         ref double[] stprob,
                                         ref int ncat,
                                         ref double[] dprob)
        {
            GetOutputAvgCurves(ref nfr, ref prob, ref qprob, ref qtprob, ref stprob, ref ncat, ref dprob);
        }

        //AEP and target Stage
        public void GetTargetAep(ref double starg, ref double pargmn, ref double pargmd)
        {
            starg = Starg;
            pargmn = Pargmn;
            pargmd = Pargmd;
        }
        public void GetTargetStageData(ref double starg, ref double pargmn, ref double pargmd)
        {
            starg = Starg;
            pargmn = Pargmn;
            pargmd = Pargmd;
        }

        //Conditional Non-Exceedance Data
        //public void  getOutputConditionalNonExceedance(long nevent,
        //                                            double[] pevent,
        //                                            long    nevstp,
        //                                            double[] sevent,
        //                                            double[] targev);

        public void GetOutputConditionalNonExceedance(ref int nevent,
                                                     ref double[] pevent,
                                                     ref int nevstp,
                                                     ref double[] sevent,
                                                     ref double[] targev)
        {
            int i = 0;
            nevent = Nevent;
            nevstp = Nevstp;

            pevent = new double[Nevent];
            sevent = new double[Nevstp * Nevent];
            targev = new double[Nevent];

            for (i = 0; i < Nevent; i++)
            {
                pevent[i] = Pevent[i];
                targev[i] = Targev[i];
            }
            for (i = 0; i < Nevstp * Nevent; i++) sevent[i] = Sevent[i];
        }

        //Risk Analysis Data
        public void GetOutputLongTermRisk(ref int nrisk,
                                        ref double[] targyr,
                                        ref double[] targrk)
        {
            int i = 0;
            nrisk = Nrisk;
            targyr = new double[Nrisk];
            targrk = new double[Nrisk];

            for (i = 0; i < Nrisk; i++)
            {
                targyr[i] = Targyr[i];
                targrk[i] = Targrk[i];
            }
        }

        //EAD
        public void GetEad(ref int ncat, ref int[] idCat, ref double[] ead)
        {
            int i = 0;
            ncat = Ncat;
            idCat = new int[Ncat + 1];
            ead = new double[Ncat + 1];

            for (i = 0; i < Ncat + 1; i++)
            {
                idCat[i] = -1;
                ead[i] = 0.0;
            }
            for (i = 0; i < Ncat; i++)
            {
                idCat[i] = IdCat[i];
                ead[i] = Ead[i];
                ead[Ncat] += ead[i];
            }
        }
        public void GetEadDistrib(ref int ncalc, ref double[] pcalc, ref double[] cead)
        {
            int i = 0;
            ncalc = Ncalc;
            pcalc = new double[Ncalc];
            cead = new double[Ncalc];

            for (i = 0; i < Ncalc; i++)
            {
                pcalc[i] = Pcalc[i];
                cead[i] = Cead[i];
            }
        }
        public void GetEadDistribTable(ref int netbl, ref double[] ptead, ref double[] ctead)
        {
            int i = 0;
            netbl = NetBl;
            ptead = new double[NetBl];
            ctead = new double[NetBl];

            for (i = 0; i < NetBl; i++)
            {
                ptead[i] = Ptead[i];
                ctead[i] = Ctead[i];
            }
        }


        //Extended Distribution tables
        public void GetEadDistExTable(ref int kclassEad,
                                    ref int numSimulations,
                                    ref int numClassEad,
                                    ref int[] nhitEad,
                                    ref double[] eadClass,
                                    ref double[] eadFreq,
                                    ref double[] eadFreqI)
        {
            int i = 0;
            kclassEad = 0;
            numSimulations = NumSimulations;
            numClassEad = NumClassEad;

            if (NumClassEad > 0)
            {
                kclassEad = NumClassEad + 1;
                nhitEad = new int[kclassEad];
                eadClass = new double[kclassEad];
                eadFreq = new double[kclassEad];
                eadFreqI = new double[kclassEad];

                for (i = 0; i < numClassEad; i++)
                {
                    nhitEad[i] = NhitEad[i];
                    eadClass[i] = EadClass[i];
                    eadFreq[i] = EadFreq[i];
                    eadFreqI[i] = EadFreqI[i];
                }
            }
        }

        public void GetAepDistExTable(ref int kclassAep,
                                    ref int numSimulations,
                                    ref int numClassAep,
                                    ref int[] nhitAep,
                                    ref double[] aepClass,
                                    ref double[] aepFreq,
                                    ref double[] aepFreqI)
        {
            int i = 0;
            kclassAep = 0;
            numSimulations = NumSimulations;
            numClassAep = NumClassAep;

            if (NumClassAep > 0)
            {
                kclassAep = NumClassAep + 1;
                nhitAep = new int[kclassAep];
                aepClass = new double[kclassAep];
                aepFreq = new double[kclassAep];
                aepFreqI = new double[kclassAep];

                for (i = 0; i < NumClassAep; i++)
                {
                    nhitAep[i] = NhitAep[i];
                    aepClass[i] = AepClass[i];
                    aepFreq[i] = AepFreq[i];
                    aepFreqI[i] = AepFreqI[i];
                }
            }
        }

        public void GetAepDistStdTable(ref int nclassStd,
                                      ref double[] aepStandard,
                                      ref double[] freqStandard,
                                      ref double[] freqStandardI)
        {
            nclassStd = NclassStd;

            if (NclassStd > 0)
            {
                aepStandard = new double[NclassStd];
                freqStandard = new double[NclassStd];
                freqStandardI = new double[NclassStd];

                for (int i = 0; i < NclassStd; i++)
                {
                    aepStandard[i] = AepStandard[i];
                    freqStandard[i] = FreqStandard[i];
                    freqStandardI[i] = FreqStandardI[i];
                }
            }
        }


        public void GetDistributionResults(ref int kclassEad,
                                          ref int kclassAep,
                                          ref int numSimulations,
                                          ref int numClassEad,
                                          ref int numClassAep,
                                          ref int[] nhitEad,
                                          ref int[] nhitAep,
                                          ref double[] eadClass,
                                          ref double[] eadFreq,
                                          ref double[] aepClass,
                                          ref double[] aepFreq,
                                          ref double[] eadFreqI,
                                          ref double[] aepFreqI)
        {
            int i = 0;

            kclassEad = 0;
            kclassAep = 0;
            numSimulations = NumSimulations;
            numClassEad = NumClassEad;
            numClassAep = NumClassAep;

            if (NumClassEad > 0 && NumClassAep > 0)
            {
                kclassEad = NumClassEad + 1;
                kclassAep = NumClassAep + 1;
                nhitEad = new int[kclassEad];
                nhitAep = new int[kclassAep];
                eadClass = new double[kclassEad];
                eadFreq = new double[kclassEad];
                eadFreqI = new double[kclassEad];
                AepClass = new double[kclassAep];
                AepFreq = new double[kclassAep];
                AepFreqI = new double[kclassAep];
            }
            for (i = 0; i < NumClassEad; i++)
            {
                nhitEad[i] = NhitEad[i];
                eadClass[i] = EadClass[i];
                eadFreq[i] = EadFreq[i];
                eadFreqI[i] = EadFreqI[i];
            }
            for (i = 0; i < NumClassAep; i++)
            {
                nhitAep[i] = NhitAep[i];
                aepClass[i] = AepClass[i];
                aepFreq[i] = AepFreq[i];
                aepFreqI[i] = aepFreqI[i];
            }
        }



        //Deprecated Functions
        //Risk Analysis Data
        public void GetRiskAnalysisData(ref int nevent,
                                       ref double[] pevent,
                                       ref int nevstp,
                                       ref double[] sevent,
                                       ref int nrisk,
                                       ref double[] targyr,
                                       ref double[] targrk,
                                       ref double[] targev)
        {
            int i = 0;

            nevent = Nevent;
            pevent = new double[Nevent];
            targev = new double[Nevent];
            for (i = 0; i < Nevent; i++)
            {
                pevent[i] = Pevent[i];
                targev[i] = Targev[i];
            }

            nevstp = Nevstp;
            if(Nevstp > 0)
            {
                sevent = new double[Nevstp * Nevent];
                for (i = 0; i < Nevstp * Nevent; i++) sevent[i] = Sevent[i];
            }

            nrisk = Nrisk;
            if (Nrisk > 0)
            {
                targyr = new double[Nrisk];
                targrk = new double[Nrisk];
                for(i = 0; i < Nrisk; i++)
                {
                    targyr[i] = Targyr[i];
                    targrk[i] = Targrk[i];
                }
            }
        }

        public void GetDamageData(ref int ncat,
                                 ref int[] idCat,
                                 ref double[] ead,
                                 ref int ncalc,
                                 ref double[] cead,
                                 ref int netbl,
                                 ref double[] ptead,
                                 ref double[] ctead)
        {
            int i = 0;

            ncat = Ncat;
            idCat = new int[Ncat + 1];
            ead = new double[Ncat + 1];
            for(i = 0; i < Ncat + 1; i++)
            {
                idCat[i] = -1;
                ead[i] = 0.0;
            }
            for(i = 0; i < Ncat; i++)
            {
                idCat[i] = IdCat[i];
                ead[i] = Ead[i];
                ead[Ncat] += Ead[i];
            }

            ncalc = Ncalc;
            cead = new double[Ncalc];
            for(i = 0; i < ncalc; i++)
            {
                cead[i] = Cead[i];
            }

            netbl = NetBl;
            ptead = new double[NetBl];
            ctead = new double[NetBl];
            for(i = 0; i < NetBl; i++)
            {
                ptead[i] = Ptead[i];
                ctead[i] = Ctead[i];
            }
        }

        public void  getEAD(ref int ncat, ref int[] idCat, ref double[] ead)
        {
            int i = 0;

            ncat = Ncat;
            idCat = new int[Ncat + 1];
            ead = new double[Ncat + 1];
            for(i = 0; i < Ncat+1; i++)
            {
                idCat[i] = -1;
                ead[i] = 0.0;
            }
            for(i = 0; i < Ncat; i++)
            {
                idCat[i] = IdCat[i];
                ead[i] = Ead[i];
                ead[Ncat] += ead[i];
            }
        }

        //Miscellaneous
        public bool  IsNotStorable()
        {
            return false;   //TODO
        }
        public bool  IsNotValid()
        {
            if (IsValid)
                return false;
            else
                return true;
        }

        //Without Uncertainty Data
        public void  SetEadWoUncert(int ncat, int[] idCatWoUncert, double[] eadWoUncert)
        {
            //  Expects ncat to be number of categories + 1 to include the total category
            NcatWoUncert = ncat;
            for(int i = 0; i < ncat; i++)
            {
                IdCatWoUncert[i] = idCatWoUncert[i];
                IdCatWoUncertD[i] = (double)idCatWoUncert[i];
                EadWoUncert[i] = eadWoUncert[i];
            }
        }
        public void  GetEadWoUncert(ref int ncat, ref int[] idCatWoUncert, ref double[] eadWoUncert)
        {
            //  Expects ncat to be number of categories + 1 to include the total category
            ncat = NcatWoUncert;
            for(int i = 0; i < Ncat; i++)
            {
                idCatWoUncert[i] = IdCatWoUncert[i];
                eadWoUncert[i] = EadWoUncert[i];
            }
        }

        public void  SetProbDamgWoUncert(int ncat,
                                      int numOrds,
                                      double[] probWoUncert,
                                      double[] flowWoUncert,
                                      double[] stageWoUncert,
                                      double[] damageWoUncert)
        {
            NcatWoUncert = ncat;
            NfrWoUncert = numOrds;

            ProbWoUncert = new double[NfrWoUncert];
            FlowWoUncert = new double[NfrWoUncert];
            StageWoUncert = new double[NfrWoUncert];
            DamageWoUncert = new double[NfrWoUncert * NcatWoUncert];

            for(int i = 0; i < numOrds; i++)
            {
                ProbWoUncert[i] = probWoUncert[i];
                FlowWoUncert[i] = flowWoUncert[i];
                StageWoUncert[i] = stageWoUncert[i];
                for(int j = 0; j < ncat; j++)
                {
                    int jn = j * numOrds + i;
                    DamageWoUncert[jn] = damageWoUncert[i];
                }
            }
        }
        public void  GetProbDamgWoUncert(ref int ncat,
                                      ref int numOrds,
                                      ref double[] probWoUncert,
                                      ref double[] flowWoUncert,
                                      ref double[] stageWoUncert,
                                      ref double[] damageWoUncert)
        {
            ncat = NcatWoUncert;
            numOrds = NfrWoUncert;

            probWoUncert = new double[NfrWoUncert];
            flowWoUncert = new double[NfrWoUncert];
            stageWoUncert = new double[NfrWoUncert];
            damageWoUncert = new double[NfrWoUncert * NcatWoUncert];

            for(int i = 0; i < NfrWoUncert; i++)
            {
                probWoUncert[i] = ProbWoUncert[i];
                flowWoUncert[i] = FlowWoUncert[i];
                stageWoUncert[i] = StageWoUncert[i];
                for(int j = 0; j < Ncat; j++)
                {
                    int jn = j * NfrWoUncert + i;
                    damageWoUncert[jn] = DamageWoUncert[jn];
                }
            }
        }
        public void Print()
        {
            int i = 0, j = 0;

            WriteLine($"\nEAD Result Name: {this.Name}");
            WriteLine($"\tEAD Result ID: {this.Id}");
            WriteLine($"\tDescription: {this.Description}");
            WriteLine($"\tPlan: {PlanName}");
            WriteLine($"\tYear: {YearName}");
            WriteLine($"\tStream: {StreamName}");
            WriteLine($"\tReach: {DamageReachName}");
            WriteLine($"\tCategory Name: {this.CategoryName}");

            WriteLine($"\tIunc: {Iunc}");
            WriteLine($"\tPtarg: {Ptarg}");
            WriteLine($"\tFdmtrg: {Fdmtrg}");
            WriteLine($"\tNyears: {Nyears}");
            WriteLine($"\tDrate: {Drate}");

            //Monte Carlo Simulation Results (SIMSTATTBL)
            Print_SIMSTATTBL();

            //Array Indexing  (EADCUM_TBL)
            Print_EADCUM_TBL();

            //Simulation Average Curves  (SIMAVG_TBL)
            Print_SIMAVG_TBL();

            //Target Non-Exceedance Data  (PPNONEXTAR)
            Print_PPNONEXTAR();

            //Conditional Nonexceedance stage tables  (PPNONEXTBL)
            Print_PPNONEXTBL();

            //Long Term Risk Table  (LTRISK_TBL)
            Print_LTRISK_TBL();

            //Total EAD by Category  (SIMEAD_TBL)
            Print_SIMEAD_TBL();

            //Distribution of EAD at Standard Probabilities
            Print_EADDISTTB();

            //Distribution of EAD at Extended Probabilities
            Print_EADDISXTBL();

            //Distribution of AEP at Extended Probabilities
            Print_AEPDISXTBL();

            //Distribution of AEP at Standard Probabilities
            Print_AEPDISSTBL();

            //FDA Version Date
            WriteLine($"\n\tFda Version for this Calculation.\t{getVersionDateMethod()}");

            Write("\n");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;
            int i = 0, j = 0;

            wr.WriteLine($"\nEAD Result Name: {this.Name}");
            wr.WriteLine($"\tEAD Result ID: {this.Id}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tPlan: {PlanName}");
            wr.WriteLine($"\tYear: {YearName}");
            wr.WriteLine($"\tStream: {StreamName}");
            wr.WriteLine($"\tReach: {DamageReachName}");
            wr.WriteLine($"\tCategory Name: {this.CategoryName}");

            wr.WriteLine($"\tIunc: {Iunc}");
            wr.WriteLine($"\tPtarg: {Ptarg}");
            wr.WriteLine($"\tFdmtrg: {Fdmtrg}");
            wr.WriteLine($"\tNyears: {Nyears}");
            wr.WriteLine($"\tDrate: {Drate}");

            //Monte Carlo Simulation Results (SIMSTATTBL)
            Print_SIMSTATTBL_ToFile();

            //Array Indexing  (EADCUM_TBL)
            Print_EADCUM_TBL_ToFile();

            //Simulation Average Curves  (SIMAVG_TBL)
            Print_SIMAVG_TBL_ToFile();

            //Target Non-Exceedance Data  (PPNONEXTAR)
            Print_PPNONEXTAR_ToFile();

            //Conditional Nonexceedance stage tables  (PPNONEXTBL)
            Print_PPNONEXTBL_ToFile();

            //Long Term Risk Table  (LTRISK_TBL)
            Print_LTRISK_TBL_ToFile();

            //Total EAD by Category  (SIMEAD_TBL)
            Print_SIMEAD_TBL_ToFile();

            //Distribution of EAD at Standard Probabilities
            Print_EADDISTTB_ToFile();

            //Distribution of EAD at Extended Probabilities
            Print_EADDISXTBL_ToFile();

            //Distribution of AEP at Extended Probabilities
            Print_AEPDISXTBL_ToFile();

            //Distribution of AEP at Standard Probabilities
            Print_AEPDISSTBL_ToFile();

            //FDA Version Date
            wr.WriteLine($"\n\tFda Version for this Calculation.\t{getVersionDateMethod()}");

            wr.Write("\n");
        }
        public void Print_SIMSTATTBL()
        {
            //Monte Carlo Simulation Results (SIMSTATTBL)
            WriteLine($"\tMonte Carlo Simulation Results, Nstics: {Nstics}\t(SIMSTATTBL)");
            for (int i = 0; i < Nstics; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"{Dmean[i].ToString().PadLeft(12)}");
                Write($"{Dsdev[i].ToString().PadLeft(12)}");
                Write($"{Dskew[i].ToString().PadLeft(12)}");
                Write("\n");
            }
        }
        public void Print_EADCUM_TBL()
        {
            //Array Indexing  (EADCUM_TBL)
            WriteLine($"\n\tEad Array Indexing, Ncalc {Ncalc}\t (EADCUM_TBL)");
            for (int i = 0; i < NumRows_EadCum_Tbl; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"  {Pcalc[i].ToString("F6").PadLeft(12)}");
                Write($"  {Ntable[i].ToString("F0").PadLeft(10)}");
                Write($"  {Cead[i].ToString("F2")}");
                Write("\n");
            }
        }
        public void Print_SIMAVG_TBL()
        {
            WriteLine($"\n\tSimulation Average Curves, Number of Points: {Nfr}\t (SIMAVG_TBL)");
            for (int i = 0; i < Nfr; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"  {Prob[i].ToString("F6").PadLeft(12)}");
                Write($"  {Qprob[i].ToString("F1").PadLeft(12)}");
                Write($"  {Qtprob[i].ToString("F1").PadLeft(12)}");
                Write($"  {Stprob[i].ToString("F3").PadLeft(12)}");

                for (int j = 0; j < Ncat + 1; j++)
                {
                    int m = i * (Ncat + 1) + j;
                    //Write($"  {Dprob[m].ToString().PadLeft(12)}");
                    //Write($"  {Dprob[m].ToString("F2", en-US).PadLeft(12)}");
                    Write($"  {Dprob[m].ToString("F1").PadLeft(12)}");
                }
                Write("\n");
            }
        }
        public void Print_PPNONEXTAR()
        {
            WriteLine($"\n\tTarget Nonexceedance Data, Nevent ({Nevent})\t(PPNONEXTAR)");
            WriteLine($"\tEAD Result ID: {this.Id}");

            //for(i = 0; i < NumRows_Ppnonextar; i++)
            try
            {
                if (Nevent > 0 && Pevent.Length > 0 && Targev.Length > 0)
                {
                    for (int i = 0; i < Nevent; i++)
                    {
                        Write($"\t{i.ToString().PadLeft(5)}");
                        Write($"  {Pevent[i].ToString("F6").PadLeft(12)}");
                        Write($"  {Targev[i].ToString("F6").PadLeft(12)}");
                        Write("\n");
                    }
                }
                else
                {
                    WriteLine($"Failure to Print_PPNONEXTAR, \tEad Result ID: {this.Id}\tNevent = {Nevent}");
                }

            }
            catch (ArgumentException e)
            {
                WriteLine($"Failure to process PPNONEXTAR, Nevent: {Nevent}");
            }
        }
        public void Print_PPNONEXTBL()
        {
            WriteLine($"\n\tProject Performance table of Stages, NumRows_PPNONEXTBL ({NumRows_PPNONEXTBL}),\tNumCols_PPNONEXTBL ({NumCols_PPNONEXTBL}).");
            WriteLine($"\tEAD Result ID: {this.Id}");

            //for(int i = 0; i < NumRows_PPNONEXTBL; i++)
            try
            {
                for (int i = 0; i < Ncalc && NumRows_PPNONEXTBL > 0 && NumCols_PPNONEXTBL > 0; i++)
                {
                    Write($"\t{i.ToString().PadLeft(5)}");
                    Write($"  {Pevent[i].ToString("F6").PadLeft(12)}");
                    //for(int j = 1; j < NumCols_PPNONEXTBL; j++)
                    for (int j = 1; j < Nevent + 1; j++)
                    {
                        int m = i * (NumCols_PPNONEXTBL - 1) + j - 1;
                        Write($"  {Sevent[i].ToString("F3").PadLeft(12)}");
                    }
                    Write("\n");
                }
            }
            catch (ArgumentException e)
            {
                WriteLine($"Failure to process PPNONEXTBL, Ncalc: {Ncalc}\tNumRows: { NumRows_PPNONEXTBL} \tNumCols: {NumCols_PPNONEXTBL}");
            }
        }

        public void Print_LTRISK_TBL()
        {
            WriteLine($"\n\tLong Term Risk, Nrisk ({Nrisk})\t (LTRISK_TBL).");
            for (int i = 0; i < Nrisk; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"  {Targyr[i].ToString("F0").PadLeft(12)}");
                Write($"  {Targrk[i].ToString("F6").PadLeft(12)}");
                Write("\n");
            }
        }
        public void Print_SIMEAD_TBL()
        {
            WriteLine($"\n\tTotal EAD by Category, Ncat ({Ncat})\t(SIMEAD_TBL).");
            for (int i = 0; i < Ncat + 1; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"  {IdCat[i].ToString("F0").PadLeft(12)}");
                Write($"  {Ead[i].ToString("F2").PadLeft(12)}");
                Write("\n");
            }
        }
        public void Print_EADDISTTB()
        {
            WriteLine($"\n\tTotal EAD Distributed at Standard Probabilities, Netbl ({NetBl})\tEADDISTTB.");
            for (int i = 0; i < NetBl; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"  {Ptead[i].ToString("F6").PadLeft(12)}");
                Write($"  {Ctead[i].ToString("F2").PadLeft(12)}");
                Write("\n");
            }
        }
        public void Print_EADDISXTBL()
        { 
            WriteLine($"\n\tTotal EAD Distributed at Extended Probabilities, NumClassEad ({NumClassEad})\tEADDISXTBL.");
            for (int i = 0; i < NumClassEad; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"  {EadClass[i].ToString("F2").PadLeft(12)}");
                Write($"  {NhitEad[i].ToString("F0").PadLeft(12)}");
                Write($"  {EadFreq[i].ToString("F5").PadLeft(12)}");
                Write($"  {EadFreqI[i].ToString("F5").PadLeft(12)}");
                Write("\n");
            }
        }
        public void Print_AEPDISXTBL()
        {
            WriteLine($"\n\tTotal AEP Distributed at Extended Probabilities, NumClassAep ({NumClassAep}).");
            for (int i = 0; i < NumClassAep; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"  {AepClass[i].ToString("F5").PadLeft(12)}");
                Write($"  {NhitAep[i].ToString("F0").PadLeft(12)}");
                Write($"  {AepFreq[i].ToString("F5").PadLeft(12)}");
                Write($"  {AepFreqI[i].ToString("F5").PadLeft(12)}");
                Write("\n");
            }
        }
        public void Print_AEPDISSTBL()
        {
            WriteLine($"\n\tTotal AEP Distributed at Standard Probabilities, NclassStd ({NclassStd}).");
            for (int i = 0; i < NclassStd; i++)
            {
                Write($"\t{i.ToString().PadLeft(5)}");
                Write($"  {AepStandard[i].ToString("F5").PadLeft(12)}");
                Write($"  {FreqStandard[i].ToString("F5").PadLeft(12)}");
                Write($"  {FreqStandardI[i].ToString("F5").PadLeft(12)}");
                Write("\n");
            }
        }

        public void Print_SIMSTATTBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            //Monte Carlo Simulation Results (SIMSTATTBL)
            wr.WriteLine($"\tMonte Carlo Simulation Results, Nstics: {Nstics}\t(SIMSTATTBL)");
            for (int i = 0; i < Nstics; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"{Dmean[i].ToString().PadLeft(12)}");
                wr.Write($"{Dsdev[i].ToString().PadLeft(12)}");
                wr.Write($"{Dskew[i].ToString().PadLeft(12)}");
                wr.Write("\n");
            }
        }
        public void Print_EADCUM_TBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            //Array Indexing  (EADCUM_TBL)
            wr.WriteLine($"\n\tEad Array Indexing, Ncalc {Ncalc}\t (EADCUM_TBL)");
            for (int i = 0; i < NumRows_EadCum_Tbl; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"  {Pcalc[i].ToString("F6").PadLeft(12)}");
                wr.Write($"  {Ntable[i].ToString("F0").PadLeft(10)}");
                wr.Write($"  {Cead[i].ToString("F2")}");
                wr.Write("\n");
            }
        }
        public void Print_SIMAVG_TBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tSimulation Average Curves, Number of Points: {Nfr}\t (SIMAVG_TBL)");
            for (int i = 0; i < Nfr; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"  {Prob[i].ToString("F6").PadLeft(12)}");
                wr.Write($"  {Qprob[i].ToString("F1").PadLeft(12)}");
                wr.Write($"  {Qtprob[i].ToString("F1").PadLeft(12)}");
                wr.Write($"  {Stprob[i].ToString("F3").PadLeft(12)}");

                for (int j = 0; j < Ncat + 1; j++)
                {
                    int m = i * (Ncat + 1) + j;
                    //wr.Write($"  {Dprob[m].ToString().PadLeft(12)}");
                    //wr.Write($"  {Dprob[m].ToString("F2", en-US).PadLeft(12)}");
                    wr.Write($"  {Dprob[m].ToString("F1").PadLeft(12)}");
                }
                wr.Write("\n");
            }
        }
        public void Print_PPNONEXTAR_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tProject Performance table of Stages, NumRows_Ppnonextar ({NumRows_Ppnonextar}),\t NumCols_Ppnonextar ({NumCols_Ppnonextar}).");
            WriteLine($"\tEAD Result ID: {this.Id}");

            try
            {
                if (Nevent > 0 && Pevent.Length > 0 && Targev.Length > 0)
                {
                    for (int i = 0; i < Nevent; i++)
                    {
                        wr.Write($"\t{i.ToString().PadLeft(5)}");
                        wr.Write($"  {Pevent[i].ToString("F6").PadLeft(12)}");
                        wr.Write($"  {Targev[i].ToString("F6").PadLeft(12)}");
                        wr.Write("\n");
                    }
                }
                else
                {
                    wr.WriteLine($"Failure to Print_PPNONEXTAR_ToFile, \tEad Result ID: {this.Id}\tNevent = {Nevent}");
                    wr.WriteLine($"\tLength of Pevent: {Pevent.Length}\tTargev: {Targev.Length}");
                }
            }
            catch (ArgumentException e)
            {
                wr.WriteLine($"Failure, nevent = {Nevent}");
                wr.WriteLine($"\tId: {Id}");
                if (Pevent == null || Pevent.Length < 1) wr.WriteLine($"\t Pevent is null.");
                if (Targev == null || Targev.Length < 1) wr.WriteLine($"\t Targev is null.");
            }
        }
        public void Print_PPNONEXTBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tProject Performance table of Stages, NumRows_PPNONEXTBL ({NumRows_PPNONEXTBL}),\tNumCols_PPNONEXTBL ({NumCols_PPNONEXTBL}).");
            WriteLine($"\tEAD Result ID: {this.Id}");

            try
            {
                if (Nevent > 0 && Pevent.Length > 0 && Sevent.Length > 0)
                {
                    //for(int i = 0; i < NumRows_PPNONEXTBL; i++)
                    for (int i = 0; i < Ncalc; i++)
                    {
                        wr.Write($"\t{i.ToString().PadLeft(5)}");
                        wr.Write($"  {Pevent[i].ToString("F6").PadLeft(12)}");
                        //for(int j = 1; j < NumCols_PPNONEXTBL; j++)
                        for (int j = 1; j < Nevent + 1; j++)
                        {
                            int m = i * (NumCols_PPNONEXTBL - 1) + j - 1;
                            wr.Write($"  {Sevent[i].ToString("F3").PadLeft(12)}");
                        }
                        wr.Write("\n");
                    }
                }
                else
                {
                    wr.WriteLine($"Failure to Print_PPNONEXTBL_ToFile, \tEad Result ID: {this.Id}\tNcalc = {Ncalc}");
                    wr.WriteLine($"\tLength of Pevent: {Pevent.Length}\tSevent: {Sevent.Length}");

                }
            }
            catch (ArgumentException e)
            {
                WriteLine($"Error in Print_PPNONEXTBL_ToFile");
                WriteLine($"\tId: {Id}");
                if (Pevent.Length < 1) WriteLine($"Pevent is not dimensioned.");
                if (Sevent.Length < 1) WriteLine($"Sevent is not dimensioned.");
            }
        }

        public void Print_LTRISK_TBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tLong Term Risk, Nrisk ({Nrisk})\t (LTRISK_TBL).");
            for (int i = 0; i < Nrisk; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"  {Targyr[i].ToString("F0").PadLeft(12)}");
                wr.Write($"  {Targrk[i].ToString("F6").PadLeft(12)}");
                wr.Write("\n");
            }
        }
        public void Print_SIMEAD_TBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tTotal EAD by Category, Ncat ({Ncat})\t(SIMEAD_TBL).");
            for (int i = 0; i < Ncat + 1; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"  {IdCat[i].ToString("F0").PadLeft(12)}");
                wr.Write($"  {Ead[i].ToString("F2").PadLeft(12)}");
                wr.Write("\n");
            }
        }
        public void Print_EADDISTTB_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tTotal EAD Distributed at Standard Probabilities, Netbl ({NetBl})\tEADDISTTB.");
            for (int i = 0; i < NetBl; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"  {Ptead[i].ToString("F6").PadLeft(12)}");
                wr.Write($"  {Ctead[i].ToString("F2").PadLeft(12)}");
                wr.Write("\n");
            }
        }
        public void Print_EADDISXTBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tTotal EAD Distributed at Extended Probabilities, NumClassEad ({NumClassEad})\tEADDISXTBL.");
            for (int i = 0; i < NumClassEad; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"  {EadClass[i].ToString("F2").PadLeft(12)}");
                wr.Write($"  {NhitEad[i].ToString("F0").PadLeft(12)}");
                wr.Write($"  {EadFreq[i].ToString("F5").PadLeft(12)}");
                wr.Write($"  {EadFreqI[i].ToString("F5").PadLeft(12)}");
                wr.Write("\n");
            }
        }
        public void Print_AEPDISXTBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tTotal AEP Distributed at Extended Probabilities, NumClassAep ({NumClassAep}).");
            for (int i = 0; i < NumClassAep; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"  {AepClass[i].ToString("F5").PadLeft(12)}");
                wr.Write($"  {NhitAep[i].ToString("F0").PadLeft(12)}");
                wr.Write($"  {AepFreq[i].ToString("F5").PadLeft(12)}");
                wr.Write($"  {AepFreqI[i].ToString("F5").PadLeft(12)}");
                wr.Write("\n");
            }
        }
        public void Print_AEPDISSTBL_ToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\n\tTotal AEP Distributed at Standard Probabilities, NclassStd ({NclassStd}).");
            for (int i = 0; i < NclassStd; i++)
            {
                wr.Write($"\t{i.ToString().PadLeft(5)}");
                wr.Write($"  {AepStandard[i].ToString("F5").PadLeft(12)}");
                wr.Write($"  {FreqStandard[i].ToString("F5").PadLeft(12)}");
                wr.Write($"  {FreqStandardI[i].ToString("F5").PadLeft(12)}");
                wr.Write("\n");
            }
        }

        #endregion

    }


}
