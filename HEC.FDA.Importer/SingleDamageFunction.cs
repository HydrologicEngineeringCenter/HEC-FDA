using System;
using System.Linq;

namespace Importer
{
    [Serializable]
    public class SingleDamageFunction 
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Properties
        //originally private. public for testing
        public int NumOrdinatesAlloc { get; set; } = 0;
        //originally private. public for testing
        public ErrorType ErrorType { get; set; } = ErrorType.NONE;
        //originally private. public for testing
        public bool DirectDollar { get; set; } = false;
        //originally private. public for testing
        public int NumOrdinates { get; set; } = 0;
        public double[] Depth { get; set; }
        public double[] Damage { get; set; }
        public double[] StdDev { get; set; }
        public double[] ErrHi { get; set; }

        #endregion
        #region Constructors
        public SingleDamageFunction()
        {
            Reset();
        }
        public SingleDamageFunction(int numOrdsAlloc)
        {
            Reset(numOrdsAlloc);
        }
        #endregion
        #region Voids
        public void Reset(int kords = 50)
        {
            NumOrdinates = 0;
            NumOrdinatesAlloc = kords;
            ErrorType = ErrorType.NONE;
            DirectDollar = false;
            Depth = new double[NumOrdinatesAlloc];
            Damage = new double[NumOrdinatesAlloc];
            StdDev = new double[NumOrdinatesAlloc];
            ErrHi = new double[NumOrdinatesAlloc];
            for (int i = 0; i < NumOrdinatesAlloc; i++)
            {
                Depth[i] = 0.0;
                Damage[i] = 0.0;
                StdDev[i] = 0.0;
                ErrHi[i] = 0.0;
            }
        }
        public void ReallocateWithoutSave(int numOrdsNew)
        {
            Reset(numOrdsNew);
        }
        public void ReallocateWithSave(int numOrdsNew)
        {
            if(numOrdsNew != NumOrdinatesAlloc)
            {
                double[] depth = new double[numOrdsNew];
                double[] damage = new double[numOrdsNew];
                double[] stdDev = new double[numOrdsNew];
                double[] errHi = new double[numOrdsNew];
                for(int i = 0; i < NumOrdinates; i++)
                {
                    depth[i] = Depth[i];
                    damage[i] = Damage[i];
                    stdDev[i] = StdDev[i];
                    errHi[i] = ErrHi[i];
                }
                NumOrdinatesAlloc = numOrdsNew;
                Depth = depth;
                Damage = damage;
                StdDev = stdDev;
                ErrHi = errHi;
            }
        }
        public void AllocateOrds(int numOrdsNew)
        {
            Reset(numOrdsNew);
        }
        public void SetNumRows(int numOrdinates, ErrorType errorType)
        {
            if (numOrdinates != NumOrdinatesAlloc)
            {
                ReallocateWithSave(numOrdinates);
            }
            this.NumOrdinates = numOrdinates;
            this.ErrorType = errorType;
        }
        public void SetType(ErrorType errorType = ErrorType.NONE)
        { this.ErrorType = errorType; }
        public void SetDepth(double[] depth)
        {
            int numOrds = depth.Length;
            if (numOrds > NumOrdinatesAlloc)
                ReallocateWithSave(numOrds);
            for (int i = 0; i < numOrds; i++)
                this.Depth[i] = depth[i];
        }
        public void SetDamage(double[] damage)
        {
            int numOrds = damage.Length;
            if (numOrds > NumOrdinatesAlloc)
                ReallocateWithSave(numOrds);
            for (int i = 0; i < numOrds; i++)
                this.Damage[i] = damage[i];
        }
        public void SetStdDev(double[] stdDev)
        {
            int numOrds = stdDev.Length;
            if (numOrds > NumOrdinatesAlloc)
                ReallocateWithSave(numOrds);
            for (int i = 0; i < numOrds; i++)
                this.StdDev[i] = stdDev[i];
        }
        public void SetTriangularLower(double[] errLo)
        { SetStdDev(errLo); }
        public void SetTriangularUpper(double[] errHi)
        {
            int numOrds = errHi.Length;
            if (numOrds > NumOrdinatesAlloc)
                ReallocateWithSave(numOrds);
            for (int i = 0; i < numOrds; i++)
                this.ErrHi[i] = errHi[i];
        }
        public void SetDamageFunction(int numOrds,
                                      ErrorType errorType,
                                      bool directDollar,
                                      double[] depth,
                                      double[] damage,
                                      double[] stdDev,
                                      double[] errHi)
        {
            SetNumRows(numOrds, errorType);
            DirectDollar = directDollar;
            SetDepth(depth);
            SetDamage(damage);
            SetStdDev(stdDev);
            SetTriangularUpper(errHi);
        }
        #endregion
        #region Functions

        public int GetNumRows()
        { return NumOrdinates; }
        public void SetNumRows(int numRows)
        { NumOrdinates = numRows; }
        public ErrorType GetTypeError()
        { return ErrorType; }
        public bool IsEqualTo(SingleDamageFunction other)
        {
            if (other == null)
                return false;

            if (NumOrdinatesAlloc != other.NumOrdinatesAlloc)
                return false;

            if (ErrorType != other.ErrorType)
                return false;

            if (DirectDollar != other.DirectDollar)
                return false;

            if (NumOrdinates != other.NumOrdinates)
                return false;

            if (!Enumerable.SequenceEqual(Depth, other.Depth))
                return false;

            if (!Enumerable.SequenceEqual(Damage, other.Damage))
                return false;

            if (!Enumerable.SequenceEqual(StdDev, other.StdDev))
                return false;

            if (!Enumerable.SequenceEqual(ErrHi, other.ErrHi))
                return false;

            return true;
        }
        #endregion
    }
}
