using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryBooks.Helper
{
    public class Functions
    {
        public static int CalculateTotalPages(long numberOfRecords, Int32 pageSize)
        {
            long result;   
            int totalPages;
            Math.DivRem(numberOfRecords, pageSize, out result);
            if (result > 0)
                totalPages = (int)((numberOfRecords / pageSize)) + 1;
            else
                totalPages = (int)(numberOfRecords / pageSize);
            return totalPages;
        }     

    }
}
