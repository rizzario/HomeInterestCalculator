using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeInterestCalculator
{
    public partial class TableForm : Form
    {
        public int itotalMonth { get; set; }
        public float housePrice {  get; set; }

        public float downPayment { get; set; }
        public float loanAmount { get; set; }

        public float firstStablePercent { get; set; }
        public int firstStableMonth { get; set; }

        public float secondStablePercent { get; set; }
        public int secondStableMonth { get; set; }

        public float thirdStablePercent { get; set; }
        public int thirdStableMonth { get; set; }

        public float mrr_rate { get; set; }

        public bool stablestatus { get; set; }
        public int stableperiod { get; set; }

        public DateTime startPeriod { get; set; }

        public int floatingMonth;
        public float firstpayment, secondpayment, thirdpayment, lastpayment;
        public float firstinterest, secondinterest, thirdinterest;
        public float firstprincipal, secondprincipal, thirdprincipal;
        public float firstinterestamt, firstloanamt, firsttotalamt;
        public float secondinterestamt, secondloanamt, secondtotalamt;
        public float thirdinterestamt, thirdloanamt, thirdtotalamt;
        public float lastinterestamt, lastloanamt, lasttotalamt;
        public float firstinitialloan, secondinitialloan, thirdinitialloan, lastinitialloan;

        public DateTime firstPeriod, secondPeriod, thirdPeriod, lastPeriod;

        public float overallTotalamt, overallInterest, overallInterestamt;

        public TableForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            housePriceAnsLabel.Text = housePrice.ToString("#,##0.00", CultureInfo.InvariantCulture);
            loanTermAnsLabel.Text = (itotalMonth / 12).ToString();
            downPaymentAnsLabel.Text = downPayment.ToString("#,##0.00", CultureInfo.InvariantCulture);
            loanAmtAnsLabel.Text = loanAmount.ToString("#,##0.00", CultureInfo.InvariantCulture);
        }

        // Calculate InterestPayment
        public float InterestPayment(float loanAmt, float interest, DateTime dateStart)
        {
            // Result for return
            float interestPayment;

            // Check date with Leap Years
            int total_day = CheckLeapYear(dateStart);
            int previous_month = dateStart.Month - 1;
            if (previous_month == 0) { previous_month = 12; }  // New years calculate
            int dayofMonth = DateTime.DaysInMonth(dateStart.Year, previous_month); //Get total days in Month

            // Interest Calculate
            float annualrate = interest / 100;
            float interestrate = (annualrate / total_day) * dayofMonth;

            //Result
            interestPayment = loanAmt * interestrate;

            return interestPayment;
        }

        private int CheckLeapYear(DateTime date)
        {
            int amount_day = 365;
            if (date.Year % 4 == 0)
            {
                if (!(date.Year % 400 == 0) & (date.Year % 100 == 0)) { amount_day = 365; }
                else if (date.Year % 400 == 0) { amount_day = 366; }
                else { amount_day = 366; }
            }
            return amount_day;
        }

        // Calculate Payment per month
        public float MonthlyPayment
            (float loanAmt, 
            float interest, 
            DateTime dateStart, 
            int monthPeriod)
        {
            // Result for return
            float monthlyPayment;

            // Check date with Leap Years
            int total_day = CheckLeapYear(dateStart);
            int getMonth = dateStart.Month;
            int dayofMonth = DateTime.DaysInMonth(dateStart.Year, getMonth); //Get total days in Month

            // Monthly Payment Calculate
            float annualrate = interest / 100;
            float interestrate = (annualrate / total_day) * dayofMonth;

            //Result monthly payment
            monthlyPayment = (loanAmt * interestrate *
            (float)(Math.Pow(1 + interestrate, (double)(monthPeriod)))) /
            ((float)(Math.Pow(1 + interestrate, (double)(monthPeriod)) - 1));

            return monthlyPayment;
        }


        // Calculate All payment amount
        public void Calculation_Recursive
            (in int Inputmonth,
            in float InputLoanAmt,
            in float InputPayment,
            in float InputInterestRate,
            in DateTime InputPeriod, 
            out float OutputInterestAmt,
            out float OutputLoanAmt)
        {
            float Principal, Interest;
            OutputLoanAmt = InputLoanAmt;
            OutputInterestAmt = 0;
            // Get Interest from total month
            for (int i = 1; i <= Inputmonth; i++)
            {
                Interest = InterestPayment(OutputLoanAmt, InputInterestRate, InputPeriod.AddMonths(i));
                Principal = InputPayment - Interest;
                OutputInterestAmt += Interest;
                if (OutputLoanAmt - Principal <= 0)
                {
                    break;
                }
                OutputLoanAmt -= Principal;
                Console.WriteLine("งวดที่: " + i.ToString() + " ดอกเบี้ย: " + Interest.ToString() + " เงินต้น: " + Principal.ToString() + " ยอดหนี้คงเหลือ: " + OutputLoanAmt.ToString());
            }
        }

        public void Value_Calculation()
        {
            lastPeriod = startPeriod;
            floatingMonth = itotalMonth;
            // Calculate floating and stable period
            if (stablestatus == false)
            {
                // Only floating period
                lastloanamt = loanAmount;
                lastinitialloan = loanAmount;
                lasttotalamt = lastloanamt;
                lastpayment = MonthlyPayment(lastloanamt, mrr_rate, lastPeriod, floatingMonth);
                Calculation_Recursive(floatingMonth, lastloanamt, lastpayment, mrr_rate, lastPeriod,out lastinterestamt,out lastloanamt);
                lasttotalamt += lastinterestamt;
            }
            else
            {
                switch (stableperiod)
                {
                    case 1:
                        // Calculate first stable period
                        firstloanamt = loanAmount;
                        firstinitialloan = loanAmount;
                        firstinterest = firstStablePercent;
                        firsttotalamt = loanAmount;
                        firstpayment = MonthlyPayment(firstloanamt, firstStablePercent, lastPeriod, floatingMonth);
                        Calculation_Recursive(firstStableMonth, loanAmount, firstpayment, firstStablePercent, lastPeriod, out firstinterestamt, out firstloanamt);
                        firsttotalamt += firstinterestamt;
                        // Calculate floating period
                        floatingMonth -= firstStableMonth;
                        lastPeriod.AddMonths(firstStableMonth);
                        lastloanamt = firstloanamt;
                        lastinitialloan = firstloanamt;
                        lasttotalamt = firsttotalamt;
                        lastpayment = MonthlyPayment(lastloanamt, mrr_rate, lastPeriod, floatingMonth);
                        Calculation_Recursive(floatingMonth, lastloanamt, lastpayment, mrr_rate, lastPeriod, out lastinterestamt, out lastloanamt);
                        lasttotalamt += lastinterestamt;
                        break;
                    case 2:
                        // Calculate first stable period
                        firstloanamt = loanAmount;
                        firstinitialloan = loanAmount;
                        firstinterest = firstStablePercent;
                        firsttotalamt = loanAmount;
                        firstpayment = MonthlyPayment(firstloanamt, firstStablePercent, lastPeriod, floatingMonth);
                        Calculation_Recursive(firstStableMonth, loanAmount, firstpayment, firstStablePercent, lastPeriod, out firstinterestamt, out firstloanamt);
                        firsttotalamt += firstinterestamt;
                        // Calculate second stable period
                        floatingMonth -= firstStableMonth;
                        secondloanamt = firstloanamt;
                        secondinitialloan = firstloanamt;
                        secondinterest = secondStablePercent;
                        secondtotalamt = firsttotalamt;
                        lastPeriod.AddMonths(firstStableMonth);
                        secondpayment = MonthlyPayment(secondloanamt, secondStablePercent, lastPeriod, floatingMonth);
                        Calculation_Recursive(secondStableMonth, firstloanamt, secondpayment, secondStablePercent, lastPeriod, out secondinterestamt, out secondloanamt);
                        secondtotalamt += secondinterestamt;
                        // Calculate floating period
                        floatingMonth -= secondStableMonth;
                        lastPeriod.AddMonths(secondStableMonth);
                        lastloanamt = secondloanamt;
                        lastinitialloan = secondloanamt;
                        lasttotalamt = secondtotalamt;
                        lastpayment = MonthlyPayment(lastloanamt, mrr_rate, lastPeriod, floatingMonth);
                        Calculation_Recursive(floatingMonth, lastloanamt, lastpayment, mrr_rate, lastPeriod, out lastinterestamt, out lastloanamt);
                        lasttotalamt += lastinterestamt;
                        break;
                    case 3:
                        // Calculate first stable period
                        firstloanamt = loanAmount;
                        firstinitialloan = loanAmount;
                        firstinterest = firstStablePercent;
                        firsttotalamt = loanAmount;
                        firstpayment = MonthlyPayment(firstloanamt, firstStablePercent, lastPeriod, floatingMonth);
                        Calculation_Recursive(firstStableMonth, loanAmount, firstpayment, firstStablePercent, lastPeriod, out firstinterestamt, out firstloanamt);
                        firsttotalamt += firstinterestamt;
                        // Calculate second stable period
                        floatingMonth -= firstStableMonth;
                        secondloanamt = firstloanamt;
                        secondinitialloan = firstloanamt;
                        secondinterest = secondStablePercent;
                        secondtotalamt = firsttotalamt;
                        lastPeriod.AddMonths(firstStableMonth);
                        secondpayment = MonthlyPayment(secondloanamt, secondStablePercent, lastPeriod, floatingMonth);
                        Calculation_Recursive(secondStableMonth, firstloanamt, secondpayment, secondStablePercent, lastPeriod, out secondinterestamt, out secondloanamt);
                        secondtotalamt += secondinterestamt;
                        // Calculate third stable period
                        floatingMonth -= secondStableMonth;
                        thirdloanamt = secondloanamt;
                        thirdinitialloan = secondloanamt;
                        thirdinterest = thirdStablePercent;
                        thirdtotalamt = secondtotalamt;
                        lastPeriod.AddMonths(secondStableMonth);
                        thirdpayment = MonthlyPayment(thirdloanamt, thirdStablePercent, lastPeriod, floatingMonth);
                        Calculation_Recursive(thirdStableMonth, secondloanamt, thirdpayment, thirdStablePercent, lastPeriod, out thirdinterestamt, out thirdloanamt);
                        thirdtotalamt += thirdinterestamt;
                        // Calculate floating period
                        floatingMonth -= thirdStableMonth;
                        lastPeriod.AddMonths(thirdStableMonth);
                        lastloanamt = thirdloanamt;
                        lastinitialloan = thirdloanamt;
                        lasttotalamt = thirdtotalamt;
                        lastpayment = MonthlyPayment(lastloanamt, mrr_rate, lastPeriod, floatingMonth);
                        Calculation_Recursive(floatingMonth, lastloanamt, lastpayment, mrr_rate, lastPeriod, out lastinterestamt, out lastloanamt);
                        lasttotalamt += lastinterestamt;
                        break;
                }

            }
        }

        private void TableForm_Load(object sender, EventArgs e)
        {
            int prototypeLocationY = addResultGroupBox.Location.Y;
            int prototypeLocationX = addResultGroupBox.Location.X;

            // Get floating month period
            Value_Calculation();

            if (stablestatus == false)
            {
                GroupBox newGroupBox = CloneGroupBox(addResultGroupBox, 
                                                     "Floating",
                                                     lastinitialloan,
                                                     floatingMonth,
                                                     mrr_rate,
                                                     lastpayment,
                                                     lastinterestamt,
                                                     lasttotalamt);

                newGroupBox.Top = prototypeLocationY;
                newGroupBox.Left = prototypeLocationX;

                this.Controls.Add(newGroupBox);
                newGroupBox.Visible = true;
            }
            else
            {
                switch (stableperiod)
                {
                    case 1:
                        GroupBox newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "First",
                                                     firstinitialloan,
                                                     firstStableMonth,
                                                     firstStablePercent,
                                                     firstpayment,
                                                     firstinterestamt,
                                                     firsttotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;

                        prototypeLocationY += newGroupBox.Height + 10;
                        newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "Floating",
                                                     lastinitialloan,
                                                     floatingMonth,
                                                     mrr_rate,
                                                     lastpayment,
                                                     lastinterestamt,
                                                     lasttotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;
                        prototypeLocationY += newGroupBox.Height + 10;

                        break;
                    case 2:
                        newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "First",
                                                     firstinitialloan,
                                                     firstStableMonth,
                                                     firstStablePercent,
                                                     firstpayment,
                                                     firstinterestamt,
                                                     firsttotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;

                        prototypeLocationY += newGroupBox.Height + 10;
                        newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "Second",
                                                     secondinitialloan,
                                                     secondStableMonth,
                                                     secondStablePercent,
                                                     secondpayment,
                                                     secondinterestamt,
                                                     secondtotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;

                        prototypeLocationY += newGroupBox.Height + 10;
                        newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "Floating",
                                                     lastinitialloan,
                                                     floatingMonth,
                                                     mrr_rate,
                                                     lastpayment,
                                                     lastinterestamt,
                                                     lasttotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;
                        prototypeLocationY += newGroupBox.Height + 10;

                        break;
                    case 3:
                        newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "First",
                                                     firstinitialloan,
                                                     firstStableMonth,
                                                     firstStablePercent,
                                                     firstpayment,
                                                     firstinterestamt,
                                                     firsttotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;

                        prototypeLocationY += newGroupBox.Height + 10;
                        newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "Second",
                                                     secondinitialloan,
                                                     secondStableMonth,
                                                     secondStablePercent,
                                                     secondpayment,
                                                     secondinterestamt,
                                                     secondtotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;

                        prototypeLocationY += newGroupBox.Height + 10;
                        newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "Third",
                                                     thirdinitialloan,
                                                     thirdStableMonth,
                                                     thirdStablePercent,
                                                     thirdpayment,
                                                     thirdinterestamt,
                                                     thirdtotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;

                        prototypeLocationY += newGroupBox.Height + 10;
                        newGroupBox = CloneGroupBox(addResultGroupBox,
                                                     "Floating",
                                                     lastinitialloan,
                                                     floatingMonth,
                                                     mrr_rate,
                                                     lastpayment,
                                                     lastinterestamt,
                                                     lasttotalamt);
                        newGroupBox.Top = prototypeLocationY;
                        newGroupBox.Left = prototypeLocationX;

                        this.Controls.Add(newGroupBox);
                        newGroupBox.Visible = true;
                        prototypeLocationY += newGroupBox.Height + 10;
                        break;
                }

            }

            overallTotalamt = lasttotalamt;
            overallInterest = ((firstStablePercent * firstStableMonth) + 
                                 (secondStablePercent * secondStableMonth) +
                                 (thirdStablePercent * thirdStableMonth) + 
                                 (mrr_rate * floatingMonth)) / itotalMonth;
            overallInterestamt = firstinterestamt + secondinterestamt + thirdinterestamt + lastinterestamt;

            totalwithIntAmtAnsLabel.Text = overallTotalamt.ToString("#,##0.00", CultureInfo.InvariantCulture);
            InterestRateAnsLabel.Text = overallInterest.ToString();
            interestAmtAnslabel.Text = overallInterestamt.ToString("#,##0.00", CultureInfo.InvariantCulture);
        }

        private GroupBox CloneGroupBox
            (GroupBox groupBox, 
            string name, 
            float loanamt, 
            int month, 
            float percent, 
            float payment, 
            float interestamt,
            float totalamt)
        {
            // Create GroupBox instance
            GroupBox newGroupBox = new GroupBox
            {
                Name = name,
                Text = groupBox.Text.Replace("{0}", name),
                Size = groupBox.Size,
                BackColor = groupBox.BackColor,
                ForeColor = groupBox.ForeColor,
                Font = groupBox.Font,
                Location = groupBox.Location
            };

            // Clone all elements inside groupBox
            foreach (Control control in groupBox.Controls)
            {
                Control cloneControl;
                switch (control.Name)
                {
                    case "addloanAmtAnsLabel":
                        cloneControl = CloneControl(control,name, loanamt.ToString("#,##0.00", CultureInfo.InvariantCulture), true);
                        newGroupBox.Controls.Add(cloneControl);
                        break;
                    case "addLoanTermAnsLabel":
                        cloneControl = CloneControl(control, name, month.ToString(), true);
                        newGroupBox.Controls.Add(cloneControl);
                        break;
                    case "addInterestRateAnsLabel":
                        cloneControl = CloneControl(control, name, percent.ToString("#,##0.00", CultureInfo.InvariantCulture), true);
                        newGroupBox.Controls.Add(cloneControl);
                        break;
                    case "addPaymentAmtAnslabel":
                        cloneControl = CloneControl(control, name, payment.ToString("#,##0.00", CultureInfo.InvariantCulture), true);
                        newGroupBox.Controls.Add(cloneControl);
                        break;
                    case "addInterestAmtAnslabel":
                        cloneControl = CloneControl(control, name, interestamt.ToString("#,##0.00", CultureInfo.InvariantCulture), true);
                        newGroupBox.Controls.Add(cloneControl);
                        break;
                    case "addTotalwithIntAmtAnsLabel":
                        cloneControl = CloneControl(control, name, totalamt.ToString("#,##0.00", CultureInfo.InvariantCulture), true);
                        newGroupBox.Controls.Add(cloneControl);
                        break;
                    default:
                        cloneControl = CloneControl(control, name, control.Text, false);
                        newGroupBox.Controls.Add(cloneControl);
                        break;
                }
            }

            return newGroupBox;
        }

        private Control CloneControl(Control control, string name, string text, bool isAns)
        {
            Control newControl = (Control) Activator.CreateInstance(control.GetType());

            // Copy common properties
            newControl.Name = control.Name.Replace("add", name); // Optional: Adjust name if necessary
            newControl.Text = text;
            newControl.Size = control.Size;
            newControl.Location = control.Location;
            newControl.Font = control.Font;
            newControl.BackColor = control.BackColor;
            newControl.ForeColor = control.ForeColor;

            if (isAns)
            {
                ((Label)newControl).TextAlign = ContentAlignment.TopRight;
            }

            return newControl;
        }
    }
}