using System;

namespace Importer
{
    [Serializable]
    public class SingleDamageFunction 
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields

        private int _NumOrdinates = 0;
        private int _NumOrdiniatesAlloc = 0;
        private ErrorType _ErrorType = ErrorType.NONE;
        private bool _DirectDollar = false;
        private double[] _Depth;
        private double[] _Damage;
        private double[] _StdDev;
        private double[] _ErrHi;

        #endregion
        #region Properties
        public double[] Depth { get => _Depth; set => _Depth = value; }
        public double[] Damage { get => _Damage; set => _Damage = value; }
        public double[] StdDev { get => _StdDev; set => _StdDev = value; }
        public double[] ErrHi { get => _ErrHi; set => _ErrHi = value; }

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
            _NumOrdinates = 0;
            _NumOrdiniatesAlloc = kords;
            _ErrorType = ErrorType.NONE;
            _DirectDollar = false;
            Depth = new double[_NumOrdiniatesAlloc];
            Damage = new double[_NumOrdiniatesAlloc];
            StdDev = new double[_NumOrdiniatesAlloc];
            ErrHi = new double[_NumOrdiniatesAlloc];
            for (int i = 0; i < _NumOrdiniatesAlloc; i++)
            {
                Depth[i] = 0.0;
                Damage[i] = 0.0;
                StdDev[i] = 0.0;
                ErrHi[i] = 0.0;
            }
        }
        public void ResetToZero()
        {
            _NumOrdinates = 0;
            _ErrorType = ErrorType.NONE;
            for (int i = 0; i < _NumOrdiniatesAlloc; i++)
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
            if(numOrdsNew != _NumOrdiniatesAlloc)
            {
                double[] depth = new double[numOrdsNew];
                double[] damage = new double[numOrdsNew];
                double[] stdDev = new double[numOrdsNew];
                double[] errHi = new double[numOrdsNew];
                for(int i = 0; i < _NumOrdinates; i++)
                {
                    depth[i] = Depth[i];
                    damage[i] = Damage[i];
                    stdDev[i] = StdDev[i];
                    errHi[i] = ErrHi[i];
                }
                _NumOrdiniatesAlloc = numOrdsNew;
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
            if (numOrdinates != _NumOrdiniatesAlloc)
            {
                ReallocateWithSave(numOrdinates);
            }
            this._NumOrdinates = numOrdinates;
            this._ErrorType = errorType;
        }
        public void SetType(ErrorType errorType = ErrorType.NONE)
        { this._ErrorType = errorType; }
        public void SetDepth(double[] depth)
        {
            int numOrds = depth.Length;
            if (numOrds > _NumOrdiniatesAlloc)
                ReallocateWithSave(numOrds);
            for (int i = 0; i < numOrds; i++)
                this.Depth[i] = depth[i];
        }
        public void SetDamage(double[] damage)
        {
            int numOrds = damage.Length;
            if (numOrds > _NumOrdiniatesAlloc)
                ReallocateWithSave(numOrds);
            for (int i = 0; i < numOrds; i++)
                this._Damage[i] = damage[i];
        }
        public void SetStdDev(double[] stdDev)
        {
            int numOrds = stdDev.Length;
            if (numOrds > _NumOrdiniatesAlloc)
                ReallocateWithSave(numOrds);
            for (int i = 0; i < numOrds; i++)
                this._StdDev[i] = stdDev[i];
        }
        public void SetTriangularLower(double[] errLo)
        { SetStdDev(errLo); }
        public void SetTriangularUpper(double[] errHi)
        {
            int numOrds = errHi.Length;
            if (numOrds > _NumOrdiniatesAlloc)
                ReallocateWithSave(numOrds);
            for (int i = 0; i < numOrds; i++)
                this._ErrHi[i] = errHi[i];
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
        { return _NumOrdinates; }
        public void SetNumRows(int numRows)
        { _NumOrdinates = numRows; }
        public ErrorType GetTypeError()
        { return this._ErrorType; }
        public bool DirectDollar
        { get; set; }
        #endregion
    }
}
