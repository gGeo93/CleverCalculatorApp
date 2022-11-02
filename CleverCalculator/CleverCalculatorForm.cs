using System.Linq;
using CleverCalculatorLib;

namespace CleverCalculator
{
    public partial class CleverCalculatorForm : Form
    {
        #region globalPointers
        InputRelated inputRelated;
        OutputRelated outputRelated;
        #endregion
        
        #region Initialization
        public CleverCalculatorForm()
        {
            InitializeComponent();
            calculatorInput.Enabled = false;
            inputRelated = new InputRelated();
            outputRelated = new OutputRelated();
        }
        #endregion

        #region Events
        private void Input_Btn(object sender, EventArgs e)
        {
            calculatorInput.Enabled = true;
            Button inputBtn = (Button)sender;
            string inputTextFromBtn = inputBtn.Text;
            calculatorInput.Text = inputRelated.InputUpdated(inputTextFromBtn[0], calculatorInput.Text);
            calculatorInput.Enabled = false;
        }

        private void Clear_Btn(object sender, EventArgs e)
        {
            calculatorInput.Clear();
            outputList.Items.Clear();
        }

        private void Result_Btn(object sender, EventArgs e)
        {
            List<string> partsOfTheOperation = new List<string>();
            string propableExceptionMessage = string.Empty;
            
            outputRelated.MakeNumericalOperationWithPriority(calculatorInput.Text, partsOfTheOperation, ref propableExceptionMessage);
            
            if (propableExceptionMessage != string.Empty)
                MessageBox.Show(propableExceptionMessage);
            
            partsOfTheOperation.ForEach(part => { outputList.Items.Add(part); });
        }
        #endregion
    }
}