using System.Runtime.InteropServices;

namespace FdaTester.ModelTester.UnitTests
{
    public static class FortranImports
    {
        #region FortranImports
        [DllImport("FDA_LPIII_DLL.dll", EntryPoint = "lp3crv_", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern void LPIIICRV(ref int ip, ref int iw, ref int ierr, ref int idebug, ref int ipear, ref int nstand, ref int nqfr, ref int kstand, ref int kfr, ref float xm, ref float sdev, ref float skew, float[] qfit, float[] qfr, float[] qfrl, float[] dqfrl, float[] pstand, float[] pdam);
        //public static extern void LPIIICRV(ref int ip, ref int iw, ref int ierr, ref int idebug, ref int ipear, ref int nstand, ref int nqfr, ref int kstand, ref int kfr, ref float xm, ref float sdev, ref float skew, float[] qfit, ref float[] qfr, ref float[] qfrl, ref float[] dqfrl, float[] pstand, ref float[] pdam);
        [DllImport("FDA_LPIII_DLL.dll", EntryPoint = "gpear_", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern void GPEAR(ref int nqfr, ref int nchai, ref int kfreq, ref int kchai, ref float xk, ref float p, ref float xm, ref float sdevmn, ref float sdrtn1, ref float xmrmin, ref float xmrmax, float[] chia, float[] pchia, float[] xnorm, float[] pr);
        [DllImport("FDA_LPIII_DLL.dll", EntryPoint = "cead_", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern void CEAD(ref int ib, ref int ip, ref int idebug, ref int isim, ref int npts, ref int kdim, ref float dmean, float[] x, float[] p);
        #endregion
    }
}
