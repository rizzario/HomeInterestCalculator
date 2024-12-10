using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HomeInterestCalculator
{
    public partial class HomeLoanForm : Form
    {
        public HomeLoanForm()
        {
            InitializeComponent();
            IntializeEventHandler();
        }

        public int itotalMonth,stableperiod, firstprdmonth, secondprdmonth, thirdprdmonth;
        public float housePrice, loanAmount, downPayment;
        public float mrrpercentage, firstpercentage, secondpercentage, thirdpercentage;
        public bool stablestatus;
        public DateTime startPeriod;

        private string firstpercentage_str, secondpercentage_str, thirdpeercentage_str, mrrpercentage_str;

        // Initiliaze Handler
        private void IntializeEventHandler()
        {
            loanTerm_yearnumericUpDown.ValueChanged += (s, e) => CalculateValues();
            loanTerm_yeartrackBar.ValueChanged += (s, e) => CalculateValues();
            stableRadioYes.CheckedChanged += (s, e) => CalculateValues();
            stableRadioNo.CheckedChanged += (s, e) => CalculateValues();
            interestPrdTimesnumericUpDown.ValueChanged += (s, e) => CalculateValues();
            firstPrdMonthnumericUpDown.ValueChanged += (s, e) => CalculateValues();
            secondPrdMonthnumericUpDown.ValueChanged += (s, e) => CalculateValues();
            thirdPrdMonthnumericUpDown.ValueChanged += (s, e) => CalculateValues();
        }

        // Calculate Programs Values
        private void CalculateValues()
        {
            try
            {
                // Changed in LoanTerm
                loanTerm_yearnumericUpDown.Value = loanTerm_yeartrackBar.Value;
                loanTerm_yeartrackBar.Value = (int) loanTerm_yearnumericUpDown.Value;
                itotalMonth = (int) loanTerm_yearnumericUpDown.Value * 12;
                decimal stablePrdsum = 0;

                // Stable interest checked
                if (stableRadioNo.Checked)
                {
                    stablePrdsum = 0;
                    floatMonthTextBox.Text = itotalMonth.ToString();
                    floatYearTextBox.Text = ((float) itotalMonth / 12).ToString();
                    stableYearTextBox.Text = (stablePrdsum / 12).ToString();
                    periodChangeGroupBox.Enabled = false;
                    stablestatus = false;
                    ResetStablePeriodPanel();
                    ToggleGroupChange();
                }
                else
                {
                    stablestatus = true;

                    if (firstPeriodStableGroupBox.Enabled)
                    {
                        firstPrdMonthnumericUpDown.Maximum = itotalMonth;
                        stablePrdsum = firstPrdMonthnumericUpDown.Value;
                        firstprdmonth = (int) firstPrdMonthnumericUpDown.Value;
                    }
                    if (secondPeriodStableGroupBox.Enabled)
                    {
                        firstPrdMonthnumericUpDown.Maximum = (itotalMonth -  secondPrdMonthnumericUpDown.Value);
                        secondPrdMonthnumericUpDown.Maximum = (itotalMonth - firstPrdMonthnumericUpDown.Value);
                        stablePrdsum = firstPrdMonthnumericUpDown.Value + secondPrdMonthnumericUpDown.Value;
                        firstprdmonth = (int) firstPrdMonthnumericUpDown.Value;
                        secondprdmonth = (int) secondPrdMonthnumericUpDown.Value;
                    }
                    if (thirdPeriodStableGroupBox.Enabled)
                    {
                        firstPrdMonthnumericUpDown.Maximum = itotalMonth - (secondPrdMonthnumericUpDown.Value + thirdPrdMonthnumericUpDown.Value);
                        secondPrdMonthnumericUpDown.Maximum = (itotalMonth - (firstPrdMonthnumericUpDown.Value + thirdPrdMonthnumericUpDown.Value));
                        thirdPrdMonthnumericUpDown.Maximum = (itotalMonth - (firstPrdMonthnumericUpDown.Value + secondPrdMonthnumericUpDown.Value));
                        stablePrdsum = firstPrdMonthnumericUpDown.Value + secondPrdMonthnumericUpDown.Value + thirdPrdMonthnumericUpDown.Value;
                        firstprdmonth = (int) firstPrdMonthnumericUpDown.Value;
                        secondprdmonth = (int) secondPrdMonthnumericUpDown.Value;
                        thirdprdmonth = (int) thirdPrdMonthnumericUpDown.Value;
                    }
                    periodChangeGroupBox.Enabled = true;
                    stableMonthTextBox.Text = stablePrdsum.ToString();
                    stableYearTextBox.Text = ((float)(stablePrdsum / 12)).ToString();
                    floatMonthTextBox.Text = (itotalMonth - int.Parse(stableMonthTextBox.Text)).ToString();
                    floatYearTextBox.Text = (float.Parse(floatMonthTextBox.Text) / 12).ToString();
                }

            } 
            catch (Exception ex) {
                MessageBox.Show($"Error in calculation: {ex.Message}");
            }
        }
        #region "Form Load"
        private void HomeLoanForm_Load(object sender, EventArgs e)
        {
            // Initial in FormLoad

            itotalMonth = (int)loanTerm_yearnumericUpDown.Value * 12;
            floatMonthTextBox.Text = itotalMonth.ToString();
            floatYearTextBox.Text = ( float.Parse(floatMonthTextBox.Text) / 12).ToString();
            loanAmtTextBox.Text = loanAmount.ToString("#,##0.00", CultureInfo.InvariantCulture);

            // Default at stable panel
            stableRadioNo.Checked = true;
            stableMonthTextBox.Text = 0.ToString();
            stableYearTextBox.Text = 0.ToString();
            ResetStablePeriodPanel();

            // Default start Period
            startPrdDateTimePicker.Value = DateTime.Today;
            startPeriod = startPrdDateTimePicker.Value;

            // Default MRR
            mrrpercentage = float.Parse(mrrPercentTextBox.Text);

            stablestatus = false;
        }
        #endregion "Form Load"

        #region "First Stable Period Percent TextBox"
        private void firstPrdPercentTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) { e.Handled = true; }
            System.Windows.Forms.TextBox firstPeriodPercent = sender as System.Windows.Forms.TextBox;
            if ((e.KeyChar == '.') && (firstPeriodPercent.Text.Contains("."))) { e.Handled = true;  }
        }

        private void firstPrdPercentTextBox_Validating(object sender, CancelEventArgs e)
        {
            System.Windows.Forms.TextBox firstPeriodPercent = sender as System.Windows.Forms.TextBox;

            if (!decimal.TryParse(firstPeriodPercent.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) || result < 0)
            {
                MessageBox.Show("Please enter a valid positive decimal number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void firstPrdPercentTextBox_TextChanged(object sender, EventArgs e)
        {
            firstpercentage_str = ReturnFormatAsFloat(firstPrdPercentTextBox.Text);
            firstpercentage = float.Parse(firstpercentage_str);
        }

        private void firstPrdPercentTextBox_Leave(object sender, EventArgs e)
        {
            firstpercentage_str = ReturnFormatAsFloat(firstPrdPercentTextBox.Text);
            firstpercentage = float.Parse(firstpercentage_str);
        }

        #endregion "First Stable Period Percent TextBox"

        #region "Second Stable Period Percent TextBox"
        private void secondPrdPercentTextBox_TextChanged(object sender, EventArgs e)
        {
            secondpercentage_str = ReturnFormatAsFloat(secondPrdPercentTextBox.Text);
            secondpercentage = float.Parse(secondpercentage_str);
        }

        private void secondPrdPercentTextBox_Leave(object sender, EventArgs e)
        {
            secondpercentage_str = ReturnFormatAsFloat(secondPrdPercentTextBox.Text);
            secondpercentage = float.Parse(secondpercentage_str);
        }
        #endregion "Second Stable Period Percent TextBox"

        #region "Third Stable Period Percent TextBox"
        private void thirdPrdPercentTextBox_TextChanged(object sender, EventArgs e)
        {
            thirdpeercentage_str =ReturnFormatAsFloat(thirdPrdPercentTextBox.Text);
            thirdpercentage = float.Parse(thirdpeercentage_str);
        }

        private void thirdPrdPercentTextBox_Leave(object sender, EventArgs e)
        {
            thirdpeercentage_str = ReturnFormatAsFloat(thirdPrdPercentTextBox.Text);
            thirdpercentage = float.Parse(thirdpeercentage_str);
        }
        #endregion "Third Stable Period Percent TextBox"

        #region "MRR Period Percent TextBox"
        private void mrrPercentTextBox_TextChanged(object sender, EventArgs e)
        {
            mrrpercentage_str = ReturnFormatAsFloat(mrrPercentTextBox.Text);
            mrrpercentage = float.Parse(mrrpercentage_str);
        }

        private void mrrPercentTextBox_Leave(object sender, EventArgs e)
        {
            mrrpercentage_str = ReturnFormatAsFloat(mrrPercentTextBox.Text);
            mrrpercentage = float.Parse(mrrpercentage_str);
        }
        #endregion "MRR Period Percent TextBox"

        private void interestPrdTimesnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdatePeriodControls(Convert.ToInt32(interestPrdTimesnumericUpDown.Value));
            stableperiod = (int) interestPrdTimesnumericUpDown.Value;
        }

        #region "Stable Period Month NumericUpDown"
        private void interestPrdTimesnumericUpDown_EnabledChanged(object sender, EventArgs e)
        {
            UpdatePeriodControls(Convert.ToInt32(interestPrdTimesnumericUpDown.Value));
        }

        private void firstPrdMonthnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (stableRadioYes.Checked) { firstprdmonth = (int)firstPrdMonthnumericUpDown.Value; }
            else { firstprdmonth = 0; }
        }

        private void secondPrdMonthnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (stableRadioYes.Checked) { secondprdmonth = (int)secondPrdMonthnumericUpDown.Value; }
            else { secondprdmonth = 0; }
        }

        private void thirdPrdMonthnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (stableRadioYes.Checked) { thirdprdmonth = (int)thirdPrdMonthnumericUpDown.Value; }
            else { thirdprdmonth = 0; }
        }
        #endregion "Stable Period Month NumericUpDown"

        // PeriodControls
        private void UpdatePeriodControls(int periodCount)
        {
            // Enable group boxes based on period count
            firstPeriodStableGroupBox.Enabled = periodCount >= 1;
            secondPeriodStableGroupBox.Enabled = periodCount >= 2;
            thirdPeriodStableGroupBox.Enabled = periodCount == 3;

            // Configure periods
            ConfigurePeriod(
                firstPrdMonthnumericUpDown,
                1,
                periodCount == 1 ? itotalMonth : itotalMonth - (periodCount - 1)
            );

            if (periodCount >= 2)
            {
                // Calculate first period
                ConfigurePeriod(
                    firstPrdMonthnumericUpDown,
                    1,
                    itotalMonth - (periodCount - 1) // Minus 1 for have at least 1 month to calculate second period
                );
                // Calculate second period
                ConfigurePeriod(
                    secondPrdMonthnumericUpDown,
                    1,
                    itotalMonth - (int)firstPrdMonthnumericUpDown.Value
                );
            }

            if (periodCount == 3)
            {
                // Calculate first period
                ConfigurePeriod(
                    firstPrdMonthnumericUpDown,
                    1,
                    itotalMonth - periodCount
                );
                // Calculate second period
                ConfigurePeriod(
                    secondPrdMonthnumericUpDown,
                    1,
                    itotalMonth - ((int)firstPrdMonthnumericUpDown.Value + 1) // Plus 1 for left over third period
                );
                // Calculate third period
                ConfigurePeriod(
                    thirdPrdMonthnumericUpDown,
                    1,
                    itotalMonth - (int)firstPrdMonthnumericUpDown.Value - (int)secondPrdMonthnumericUpDown.Value
                );
            }
        }

        // Control Period Max/Min NumericUpDown
        private void ConfigurePeriod(NumericUpDown control, int min, int max)
        {
            control.Minimum = min;
            control.Maximum = max;
            control.Value = min;
        }

        // Toggle status change
        private void ToggleGroupChange()
        {
            firstPeriodStableGroupBox.Enabled = false;
            secondPeriodStableGroupBox.Enabled = false;
            thirdPeriodStableGroupBox.Enabled = false;
        }

        private void interestPrdTimesnumericUpDown_Leave(object sender, EventArgs e)
        {
            if (interestPrdTimesnumericUpDown.Enabled) { stableperiod = (int)interestPrdTimesnumericUpDown.Value;  }
        }

        private void stableRadioYes_CheckedChanged(object sender, EventArgs e)
        {
            stableperiod = (int)interestPrdTimesnumericUpDown.Value;
        }

        private void firstPrdMonthnumericUpDown_Leave(object sender, EventArgs e)
        {

        }

        // Reset stable interest
        private void ResetStablePeriodPanel()
        {
            firstPrdMonthnumericUpDown.Value = firstPrdMonthnumericUpDown.Minimum;
            secondPrdMonthnumericUpDown.Value = secondPrdMonthnumericUpDown.Minimum;
            thirdPrdMonthnumericUpDown.Value = thirdPrdMonthnumericUpDown.Minimum;
            interestPrdTimesnumericUpDown.Value = interestPrdTimesnumericUpDown.Minimum;
            stableMonthTextBox.Text = 0.ToString();

            firstPrdPercentTextBox.Text = "0.00".ToString();
            secondPrdPercentTextBox.Text = "0.00".ToString();
            thirdPrdPercentTextBox.Text = "0.00".ToString();
        }

        // Calculate Payment Term
        private void CalculatePayment()
        {
            housePrice = float.Parse(housePriceTextBox.Text);
            downPayment = float.Parse(downPaymentTextBox.Text);
            try
            {
                loanAmount = float.Parse(housePriceTextBox.Text) - float.Parse(downPaymentTextBox.Text);
            }
            catch
            {
                
            }
            loanAmtTextBox.Text = loanAmount.ToString("#,##0.00", CultureInfo.InvariantCulture);
        }

        #region "Loan Term Handle"
        private void loanTerm_yeartrackBar_ValueChanged(object sender, EventArgs e)
        {
            loanTerm_yearnumericUpDown.Value = loanTerm_yeartrackBar.Value;
        }

        private void loanTerm_yearnumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            loanTerm_yeartrackBar.Value = (int)loanTerm_yearnumericUpDown.Value;
        }
        #endregion "Loan Term Handle"

        #region "Calculate Amount Price"
        private void housePriceTextBox_Leave(object sender, EventArgs e)
        {
            CalculatePayment();
            housePriceTextBox.Text = ReturnFormatAsFloat(housePriceTextBox.Text);
        }

        private void downPaymentTextBox_Leave(object sender, EventArgs e)
        {
            CalculatePayment();
            downPaymentTextBox.Text = ReturnFormatAsFloat(downPaymentTextBox.Text);
        }

        private void housePriceTextBox_TextChanged(object sender, EventArgs e)
        {
            CalculatePayment();
        }

        private void downPaymentTextBox_TextChanged(object sender, EventArgs e)
        {
            CalculatePayment();
        }

        #endregion "Calculate Amount Price"

        // Convert String to Float Format As String Format
        private string ReturnFormatAsFloat(string str_amount)
        {
            float amount;
            try
            {
                amount = float.Parse(str_amount);
            } catch
            {
                amount = 0;
            }
            
            str_amount = amount.ToString("#,##0.00", CultureInfo.InvariantCulture);
            return str_amount; 
        }

        // Click to Activate Result in Second Form
        private void Calculate_Click(object sender, EventArgs e)
        {
            TableForm tableForm = new TableForm();
            // Send public parameter for calculate
            tableForm.itotalMonth = itotalMonth;
            tableForm.housePrice = housePrice;
            tableForm.downPayment = downPayment;
            tableForm.loanAmount = loanAmount;
            tableForm.firstStablePercent = firstpercentage;
            tableForm.firstStableMonth = firstprdmonth;
            tableForm.secondStablePercent = secondpercentage;
            tableForm.secondStableMonth = secondprdmonth;
            tableForm.thirdStablePercent = thirdpercentage;
            tableForm.thirdStableMonth = thirdprdmonth;
            tableForm.mrr_rate = mrrpercentage;
            tableForm.stablestatus = stablestatus;
            tableForm.stableperiod = stableperiod;
            tableForm.startPeriod = startPeriod;

            tableForm.Show();
        }

        #region "Start Period DateTimePicker"
        private void startPrdDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            startPeriod = startPrdDateTimePicker.Value;
        }

        private void startPrdDateTimePicker_Enter(object sender, EventArgs e)
        {
            startPeriod = startPrdDateTimePicker.Value;
        }
        #endregion "Start Period DateTimePicker"

    }
}
