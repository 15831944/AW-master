using System;
using System.Collections.Generic;
using System.Text;
using Infragistics.Win.UltraWinGrid;

namespace Layton.AuditWizard.Common
{
    public class DateFilterCondition : FilterCondition
    {
        public DateFilterCondition(FilterCondition originalFilterCondition)
            : base(originalFilterCondition.Column, originalFilterCondition.ComparisionOperator, originalFilterCondition.CompareValue) { }

        public override bool MeetsCriteria(UltraGridRow row)
        {
            int intValue;

            if ((Int32.TryParse(this.CompareValue.ToString(), out intValue)) && (Int32.TryParse(row.Cells[this.Column].Value.ToString(), out intValue)))
            {
                int compareValue = Convert.ToInt32(this.CompareValue);
                int rowValue = Convert.ToInt32(row.Cells[this.Column].Value);

                if (this.ComparisionOperator == FilterComparisionOperator.Equals)
                {
                    return rowValue == compareValue;
                }
                else if (this.ComparisionOperator == FilterComparisionOperator.GreaterThan)
                {
                    return rowValue > compareValue;
                }
                else if (this.ComparisionOperator == FilterComparisionOperator.GreaterThanOrEqualTo)
                {
                    return rowValue >= compareValue;
                }
                else if (this.ComparisionOperator == FilterComparisionOperator.LessThan)
                {
                    return rowValue < compareValue;
                }
                else if (this.ComparisionOperator == FilterComparisionOperator.LessThanOrEqualTo)
                {
                    return rowValue <= compareValue;
                }
                else
                {
                    return base.MeetsCriteria(row);
                }
            }
            else
            {
                return base.MeetsCriteria(row);
            }
        }
    }
}
