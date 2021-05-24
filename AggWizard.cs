#r "System.Drawing"

using System.Drawing;

// Parameters
int formWidth = 500;
int formHeight = 700;
int aggNumber = 0;
int prec = 0;
int currentPage = 0;
string aggSuffix = "AGG";
string aggTableName = string.Empty;
string baseTableName = string.Empty;
bool IsExpOrCol = false;
bool hasSaved = false;
bool darkModeEnabled = false;
string ebiURL = @"https://www.elegantbi.com";
int gap = 10;
string aggType = string.Empty;
string aggNameExport = string.Empty;
string[] aggTypeList = {"GroupBy","Sum","Count","Min","Max"};
string[] infoList = {
	 "Select the columns to include in the agg table."
	,"Here is a summary of the selected agg table."
};
string annPrefix = "TabularAggWizard_";
int annPrefixLen = annPrefix.Length;
var sb_ExportScript = new System.Text.StringBuilder();
string newline = Environment.NewLine;

// Start screen
System.Windows.Forms.Form newForm = new System.Windows.Forms.Form();
System.Windows.Forms.Panel startPanel = new System.Windows.Forms.Panel();
System.Windows.Forms.RadioButton newbatchButton = new System.Windows.Forms.RadioButton();
System.Windows.Forms.RadioButton existingbatchButton = new System.Windows.Forms.RadioButton();
System.Windows.Forms.ComboBox existingComboBox = new System.Windows.Forms.ComboBox();            
System.Windows.Forms.Button goButton = new System.Windows.Forms.Button();
System.Windows.Forms.Label homeToolLabel = new System.Windows.Forms.Label();
System.Windows.Forms.Label designedByHome = new System.Windows.Forms.Label();
System.Windows.Forms.LinkLabel ebiHome = new System.Windows.Forms.LinkLabel();
System.Net.WebClient w = new System.Net.WebClient();

// Step 1
System.Windows.Forms.Panel topPanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Panel namePanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Panel aggPanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Panel infoPanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Panel treePanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Label toolNameLabel = new System.Windows.Forms.Label();
System.Windows.Forms.Label baseNameLabel = new System.Windows.Forms.Label();
System.Windows.Forms.ComboBox baseNameComboBox = new System.Windows.Forms.ComboBox();
System.Windows.Forms.Label precLabel = new System.Windows.Forms.Label();
System.Windows.Forms.ToolTip precLabelToolTip = new System.Windows.Forms.ToolTip();
precLabelToolTip.SetToolTip(precLabel, "An agg table with a higher precedence will be considered prior to agg tables with a lower precedence.");
System.Windows.Forms.NumericUpDown precNumericUpDown = new System.Windows.Forms.NumericUpDown();
System.Windows.Forms.Label aggNameLabel = new System.Windows.Forms.Label();
System.Windows.Forms.TextBox aggNameTextBox = new System.Windows.Forms.TextBox();
System.Windows.Forms.Label infoLabel = new System.Windows.Forms.Label();
System.Windows.Forms.TreeView step1TreeView = new System.Windows.Forms.TreeView();

// Step 2
System.Windows.Forms.Panel s2namePanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Panel s2aggPanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Panel s2treePanel = new System.Windows.Forms.Panel();

System.Windows.Forms.Label s2aggLabel = new System.Windows.Forms.Label();
System.Windows.Forms.TextBox s2aggTextBox = new System.Windows.Forms.TextBox();
System.Windows.Forms.Label s2headerLabel = new System.Windows.Forms.Label();

// Step 3
System.Windows.Forms.Panel s3aggPanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Panel s3infoPanel = new System.Windows.Forms.Panel();
System.Windows.Forms.Panel s3treePanel = new System.Windows.Forms.Panel();

System.Windows.Forms.Label s3aggLabel = new System.Windows.Forms.Label();
System.Windows.Forms.ComboBox s3aggComboBox = new System.Windows.Forms.ComboBox();
System.Windows.Forms.Label s3infoLabel = new System.Windows.Forms.Label();
System.Windows.Forms.TreeView step3TreeView = new System.Windows.Forms.TreeView();

System.Windows.Forms.ImageList imageList = new System.Windows.Forms.ImageList();
System.Windows.Forms.ImageList imageList2 = new System.Windows.Forms.ImageList();
System.Windows.Forms.Button saveButton = new System.Windows.Forms.Button();
System.Windows.Forms.ToolTip saveButtonToolTip = new System.Windows.Forms.ToolTip();
saveButtonToolTip.SetToolTip(saveButton, "Saves the changes back to the model");
System.Windows.Forms.Button deleteButton = new System.Windows.Forms.Button();
System.Windows.Forms.ToolTip deleteButtonToolTip = new System.Windows.Forms.ToolTip();
deleteButtonToolTip.SetToolTip(deleteButton, "Deletes this agg table");
System.Windows.Forms.Button nextButton = new System.Windows.Forms.Button();
System.Windows.Forms.ToolTip nextButtonToolTip = new System.Windows.Forms.ToolTip();
nextButtonToolTip.SetToolTip(nextButton, "Navigate to the next page");
System.Windows.Forms.Button scriptButton = new System.Windows.Forms.Button();
System.Windows.Forms.ToolTip scriptButtonToolTip = new System.Windows.Forms.ToolTip();
scriptButtonToolTip.SetToolTip(scriptButton, "Saves a script to your desktop with the instructions to create the agg tables");
System.Windows.Forms.Button backButton = new System.Windows.Forms.Button();
System.Windows.Forms.ToolTip backButtonToolTip = new System.Windows.Forms.ToolTip();
backButtonToolTip.SetToolTip(backButton, "Navigate back a page");
System.Windows.Forms.Button recoButton = new System.Windows.Forms.Button();
System.Windows.Forms.ToolTip recoButtonToolTip = new System.Windows.Forms.ToolTip();
recoButtonToolTip.SetToolTip(recoButton, "Recommend summarizations for the aggregation columns");
//System.Windows.Forms.Label saveComment = new System.Windows.Forms.Label();

System.Windows.Forms.Label elegantLabel = new System.Windows.Forms.Label();
System.Windows.Forms.LinkLabel ebiMain = new System.Windows.Forms.LinkLabel();

// Summary Screen
System.Windows.Forms.TreeView summaryTreeView = new System.Windows.Forms.TreeView();
System.Windows.Forms.ComboBox summarytypeComboBox = new System.Windows.Forms.ComboBox();

// Colors
System.Drawing.Color sideColor =  ColorTranslator.FromHtml("#BFBFBF");
System.Drawing.Color bkgrdColor =  ColorTranslator.FromHtml("#F2F2F2");
System.Drawing.Color lighttextColor =  Color.White;
System.Drawing.Color darktextColor =  Color.Black;
System.Drawing.Color visibleColor = darktextColor;
System.Drawing.Color hiddenColor = Color.Gray;
System.Drawing.Color darkModeBack =  ColorTranslator.FromHtml("#444444");
System.Drawing.Color darkModeName =  ColorTranslator.FromHtml("#F2C811");
System.Drawing.Color darkModeText =  Color.White;

// Fonts
System.Drawing.Font toolNameFont = new Font("Century Gothic", 22);
System.Drawing.Font elegantFont = new Font("Century Gothic", 10, FontStyle.Italic);
System.Drawing.Font homeToolNameFont = new Font("Century Gothic", 24);
System.Drawing.Font stdFont = new Font("Century Gothic", 10);

// Form
newForm.TopMost = true;
newForm.BackColor = bkgrdColor;
newForm.Text = "Agg Wizard";
newForm.Size = new Size(formWidth,formHeight);
newForm.MaximumSize = new Size(formWidth,formHeight);
newForm.MinimumSize = new Size(formWidth,formHeight);
newForm.Controls.Add(startPanel);

// Step 1
newForm.Controls.Add(topPanel);
newForm.Controls.Add(namePanel);
newForm.Controls.Add(aggPanel);
newForm.Controls.Add(infoPanel);
newForm.Controls.Add(treePanel);

// Step 2
newForm.Controls.Add(s2namePanel);
newForm.Controls.Add(s2aggPanel);
newForm.Controls.Add(s2treePanel);

newForm.Controls.Add(s3aggPanel);
newForm.Controls.Add(s3infoPanel);
newForm.Controls.Add(s3treePanel);

int panelX = 0;

startPanel.Controls.Add(newbatchButton);
startPanel.Controls.Add(existingbatchButton);
startPanel.Controls.Add(existingComboBox);
startPanel.Controls.Add(goButton);
startPanel.Controls.Add(homeToolLabel);
startPanel.Controls.Add(designedByHome);
startPanel.Controls.Add(ebiHome);
startPanel.Visible = true;
startPanel.Size = new Size(formWidth,formHeight);
startPanel.Location = new Point(0,0);

// Panels
topPanel.Visible = false;
topPanel.Size = new Size(formWidth,70);
topPanel.Location = new Point(panelX,0);
topPanel.BackColor = bkgrdColor;
namePanel.Visible = false;
namePanel.Size = new Size(formWidth,50);
namePanel.Location = new Point(panelX,70);
namePanel.BackColor = bkgrdColor;
aggPanel.Visible = false;
aggPanel.Size = new Size(formWidth,50);
aggPanel.Location = new Point(panelX,120);
aggPanel.BackColor = bkgrdColor;
infoPanel.Visible = false;
infoPanel.Size = new Size(formWidth,20);
infoPanel.Location = new Point(panelX,170);
infoPanel.BackColor = bkgrdColor;
treePanel.Visible = false;
treePanel.Size = new Size(formWidth,500);
treePanel.Location = new Point(panelX,190);
treePanel.BackColor = bkgrdColor;

s2namePanel.Visible = false;
s2namePanel.Size = new Size(formWidth,30);
s2namePanel.Location = new Point(panelX,70);
s2namePanel.BackColor = bkgrdColor;

s2aggPanel.Visible = false;
s2aggPanel.Size = new Size(formWidth,20);
s2aggPanel.Location = new Point(panelX,100);
s2aggPanel.BackColor = bkgrdColor;

s2treePanel.Visible = false;
s2treePanel.Size = new Size(formWidth-36,546);
s2treePanel.Location = new Point(panelX+gap,105);
s2treePanel.BackColor = Color.White;
s2treePanel.AutoScroll = true;
s2treePanel.VerticalScroll.Enabled = true;
s2treePanel.VerticalScroll.Visible = true;
s2treePanel.HorizontalScroll.Visible = false;
s2treePanel.HorizontalScroll.Visible = false;

s3aggPanel.Visible = false;
s3aggPanel.Size = new Size(formWidth,30);
s3aggPanel.Location = new Point(panelX,70);
s3aggPanel.BackColor = bkgrdColor;

s3infoPanel.Visible = false;
s3infoPanel.Size = new Size(formWidth,20);
s3infoPanel.Location = new Point(panelX,100);
s3infoPanel.BackColor = bkgrdColor;

s3treePanel.Visible = false;
s3treePanel.Size = new Size(formWidth,540);
s3treePanel.Location = new Point(panelX,120);
s3treePanel.BackColor = bkgrdColor;

// Add items to panels
topPanel.Controls.Add(toolNameLabel);
topPanel.Controls.Add(saveButton);
topPanel.Controls.Add(deleteButton);
topPanel.Controls.Add(nextButton);
topPanel.Controls.Add(scriptButton);
topPanel.Controls.Add(backButton);
topPanel.Controls.Add(recoButton);
namePanel.Controls.Add(baseNameLabel);
namePanel.Controls.Add(baseNameComboBox);
namePanel.Controls.Add(precLabel);
namePanel.Controls.Add(precNumericUpDown);
aggPanel.Controls.Add(aggNameLabel);
aggPanel.Controls.Add(aggNameTextBox);
infoPanel.Controls.Add(infoLabel);
treePanel.Controls.Add(step1TreeView);
treePanel.Controls.Add(elegantLabel);
treePanel.Controls.Add(ebiMain);
//treePanel.Controls.Add(saveComment);

// Step 2
s2namePanel.Controls.Add(s2aggLabel);
s2namePanel.Controls.Add(s2aggTextBox);
s2aggPanel.Controls.Add(s2headerLabel);

// Step 3
s3aggPanel.Controls.Add(s3aggLabel);
s3aggPanel.Controls.Add(s3aggComboBox);
s3infoPanel.Controls.Add(s3infoLabel);
s3treePanel.Controls.Add(step3TreeView);

elegantLabel.Visible = true;
elegantLabel.Size = new Size(92,20);
elegantLabel.Location = new Point(formWidth-200,481);
elegantLabel.ForeColor = visibleColor;
elegantLabel.Text = "Designed by";
elegantLabel.Font = elegantFont;

ebiMain.Visible = true;
ebiMain.Size = new Size(100,20);
ebiMain.Location = new Point(390,481);
ebiMain.Font = elegantFont;
ebiMain.Text = "Elegant BI";

//saveComment.Visible = false;
// saveComment.Size = new Size(200)

// Start Screen Objects
homeToolLabel.Text = "Agg Wizard";
homeToolLabel.Size = new Size(450,350);
homeToolLabel.Location = new Point(145,150);
homeToolLabel.Font = homeToolNameFont;

designedByHome.Text = "Designed by";
designedByHome.Size = new Size(92,40);
designedByHome.Location = new Point(160,600);
designedByHome.Font = elegantFont;

ebiHome.Text = "Elegant BI";
ebiHome.Size = new Size(100,40);
ebiHome.Location = new Point(160+89,600);
ebiHome.Font = elegantFont;

newbatchButton.Size = new Size(250,25);
newbatchButton.Location = new Point(150,250);
newbatchButton.Text = "Create New Agg Table";
newbatchButton.Font = stdFont;

existingbatchButton.Size = new Size(250,25);
existingbatchButton.Location = new Point(150,280);
existingbatchButton.Text = "Modify Existing Agg Table";
existingbatchButton.Font = stdFont;

existingComboBox.Visible = false;
existingComboBox.Size = new Size(250,20);
existingComboBox.Location = new Point(120,320);
existingComboBox.Font = stdFont;

goButton.Visible = false;
goButton.Size = new Size(100,30);
goButton.Text = "Go";
goButton.Font = stdFont;

toolNameLabel.Visible = true;
toolNameLabel.Size = new Size(330,50);
toolNameLabel.Location = new Point(5,15);
toolNameLabel.Font = toolNameFont;
toolNameLabel.Text = "Agg Wizard";

baseNameLabel.Visible = true;
baseNameLabel.Size = new Size(140,20);
baseNameLabel.Location = new Point(10,0);
baseNameLabel.Text = "Select a table to aggregate";

baseNameComboBox.Visible = true;
baseNameComboBox.Size = new Size(250,20);
baseNameComboBox.Location = new Point(10,20);
string[] tableTypeList = {"Calculated Table", "Calculation Group Table"};

// Populate baseNameComboBox: Only fact tables; no calculated tables or calculation groups; no calculated columns; legacy partitions only; partitions are SELECT * FROM...
foreach (var t in Model.Tables.Where(a => a.UsedInRelationships.Any(b => b.FromTable.Name == a.Name) && ! a.UsedInRelationships.Any(c => c.ToTable.Name == a.Name) && a.Partitions.All(f => f.ObjectTypeName.ToString().Contains("Legacy") && f.Query.Replace(" ","").Replace("\n","").Replace("\r","").Replace("\t","").ToUpper().StartsWith("SELECT*FROM")) && ! a.Columns.Any(e => e.Type.ToString() == "Calculated") && ! a.UsedInRelationships.Any(d => d.SecurityFilteringBehavior.ToString() == "BothDirections") && ! tableTypeList.Contains(a.ObjectTypeName.ToString())).OrderBy(a => a.Name).ToList())
{
	baseNameComboBox.Items.Add(t.Name);
}

precLabel.Visible = true;
precLabel.Size = new Size(70,20);
precLabel.Location = new Point(formWidth-90,0);
precLabel.Text = "Precedence";

precNumericUpDown.Visible = true;
precNumericUpDown.Size = new Size(40,20);
precNumericUpDown.Location = new Point(formWidth-70,20);
precNumericUpDown.Enabled = false;

aggNameLabel.Visible = true;
aggNameLabel.Size = new Size(140,20);
aggNameLabel.Location = new Point(10,0);
aggNameLabel.Text = "Aggregation table";

aggNameTextBox.Visible = true;
aggNameTextBox.Size = new Size(250,20);
aggNameTextBox.Location = new Point(10,20);
aggNameTextBox.Enabled = false;

infoLabel.Visible = true;
infoLabel.Size = new Size(250,20);
infoLabel.Location = new Point(gap,0);
infoLabel.Text = infoList[0];

step1TreeView.Visible = true;
step1TreeView.Size = new Size(formWidth-35,460);
step1TreeView.Location = new Point(gap,0);
step1TreeView.BackColor = Color.White;
step1TreeView.CheckBoxes = true;

s2aggLabel.Visible = true;
s2aggLabel.Size = new Size(94,20);
s2aggLabel.Location = new Point(gap,2);
s2aggLabel.Text = "Aggregation table:";

s2aggTextBox.Visible = true;
s2aggTextBox.Size = new Size(250,20);
s2aggTextBox.Location = new Point(105,0);
s2aggTextBox.Enabled = false;

s2headerLabel.Visible = true;
s2headerLabel.Size = new Size(formWidth,20);
s2headerLabel.Location = new Point(gap,0);
s2headerLabel.Text = "Aggregation column                                                                                                  Summarization";

s3aggLabel.Visible = true;
s3aggLabel.Size = new Size(94,20);
s3aggLabel.Location = new Point(gap,2);
s3aggLabel.Text = "Aggregation table:";

s3aggComboBox.Visible = true;
s3aggComboBox.Size = new Size(250,20);
s3aggComboBox.Location = new Point(105,0);
s3aggComboBox.Enabled = true;

s3infoLabel.Visible = true;
s3infoLabel.Size = new Size(250,20);
s3infoLabel.Location = new Point(gap,0);
s3infoLabel.Text = infoList[1];

step3TreeView.Visible = true;
step3TreeView.Size = new Size(formWidth-35,530);
step3TreeView.Location = new Point(gap,0);
step3TreeView.BackColor = bkgrdColor;
step3TreeView.CheckBoxes = false;

// Add images from web to Image List
var urlPrefix = "https://raw.githubusercontent.com/m-kovalsky/Tabular/master/Icons/";
var urlSuffix = "Icon.png";

string[] imageURLList = { "Table", "Column", "SummaryTable", "SummaryColumn", "SaveDark", "Script", "Delete", "ForwardArrow", "BackArrow","RecommendDark", "SaveDarkGray", "ScriptGray", "DeleteGray", "ForwardArrowGray", "BackArrowGray", "RecommendDarkGray"};
for (int i = 0; i < imageURLList.Count(); i++)
{
    var url = urlPrefix + imageURLList[i] + urlSuffix;      
    byte[] imageByte = w.DownloadData(url);
    System.IO.MemoryStream ms = new System.IO.MemoryStream(imageByte);
    System.Drawing.Image im = System.Drawing.Image.FromStream(ms);

    if (i<4)
    {
        imageList.Images.Add(im);
    }
    else
    {
        imageList2.Images.Add(im);
    }
}   

imageList.ImageSize = new Size(16, 16); 
step1TreeView.ImageList = imageList;
step3TreeView.ImageList = imageList;
imageList2.ImageSize = new Size(23, 23); 
saveButton.ImageList = imageList2;
deleteButton.ImageList = imageList2;
nextButton.ImageList = imageList2;
scriptButton.ImageList = imageList2;
backButton.ImageList = imageList2;
recoButton.ImageList = imageList2;

// Lambda expression for all the steps to next page
System.Action<int> NextStep = stepNumber =>
{
    // Create screen 1 tree view
	if (stepNumber == 0 || stepNumber == 1)
	{
        step1TreeView.Nodes.Clear();

        foreach (var c in Model.Tables[baseTableName].Columns.Where(a => a.IsHidden == true && (a.UsedInRelationships.Count() > 0 || (a.DataType == DataType.Int64 || a.DataType == DataType.Decimal || a.DataType == DataType.Double))).OrderBy(a => a.Name).ToList())
        {
            var x = step1TreeView.Nodes.Add(c.Name);
            x.ImageIndex = 1;
            x.SelectedImageIndex = 1;

            if (c.IsHidden)
            {
                x.ForeColor = hiddenColor;
            }
            else
            {
                x.ForeColor = visibleColor;
            }

            // Update checkboxes
            if (stepNumber == 1)
            {   
                if (c.HasAnnotation(annPrefix+aggNumber))
                {
                    x.Checked = true;
                }
            }
        }
	}
    else if (stepNumber == 2)
    {
        s2aggTextBox.Text = aggTableName;
        string ann = annPrefix+aggNumber;
        int startY = 22;
        int labelYIncrement = 30;
        int i=0;

        // foreach (System.Windows.Forms.Control cCB in s2treePanel.Controls.OfType<System.Windows.Forms.ComboBox>())
        // {
        //     s2treePanel.Controls.Remove(cCB);
        // }

        // foreach (System.Windows.Forms.Control cCB in s2treePanel.Controls.OfType<System.Windows.Forms.Label>())
        // {
        //     s2treePanel.Controls.Remove(cCB);
        // }
        s2treePanel.Controls.Clear();

        foreach (var c in Model.Tables[baseTableName].Columns.Where(a => a.HasAnnotation(ann)).OrderBy(a => a.Name).ToList())
        {
            string columnName = c.Name;
            string dt = c.DataType.ToString();

            System.Windows.Forms.ComboBox aggComboBox = new System.Windows.Forms.ComboBox();
            System.Windows.Forms.Label aggColumnLabel = new System.Windows.Forms.Label();
            aggComboBox.Visible = true;
            aggComboBox.Size = new Size(90,20);
            aggComboBox.Location = new Point(365,startY+(i*labelYIncrement));
            aggComboBox.Name = columnName;
            aggComboBox.Tag = "agg";
            s2treePanel.Controls.Add(aggComboBox);
            s2treePanel.Controls.Add(aggColumnLabel);

            aggComboBox.Items.Clear();
            
            if (dt != "Decimal" && dt != "Int64" && dt != "Double")
            {
                aggComboBox.Items.Add("GroupBy");
            }
            else 
            {
                for (int o =0; o < aggTypeList.Length; o++)
                {    
                    aggComboBox.Items.Add(aggTypeList[o]);            
                }        
            }
            
            if (dt == "Decimal" || dt == "Double")
            {
                aggComboBox.Items.Remove("GroupBy");
            }

            // Show pre-existing annotation value
            if (c.HasAnnotation(ann))
            {
                aggComboBox.Text = c.GetAnnotation(ann);
            }

            aggColumnLabel.Visible = true;
            aggColumnLabel.Size = new Size(260,20);
            aggColumnLabel.Location = new Point(0,startY+(i*labelYIncrement));
            aggColumnLabel.Text = columnName;
            aggColumnLabel.Tag = "agg";      

            i++;
        }
    }
    // Create summary tree view
    else if (stepNumber == 3)
    {
        step3TreeView.Nodes.Clear();
        sb_ExportScript.Clear();

        string aggName = s3aggComboBox.Text;
        string baseTbl = aggName.Substring(0,aggName.IndexOf("_AGG_"));
        string aggNum = aggName.Substring(aggName.LastIndexOf("_")+1);
        aggNameExport = aggName;
        string ann = annPrefix+aggNum;
        string pr = Model.Tables[baseTbl].GetAnnotation(ann);

        var x = step3TreeView.Nodes.Add(baseTbl + ": Precedence " + pr);
        x.ImageIndex = 2;
        x.SelectedImageIndex = 2;

        sb_ExportScript.Append("Model.Tables[\""+baseTbl+"\"].SetAnnotation(\""+annPrefix+aggNum+"\",\""+pr+"\")" + newline + newline);

        foreach (var c in Model.Tables[baseTbl].Columns.Where(b => b.HasAnnotation(ann)).ToList())
        {
            string colName = c.Name;
            string aT = c.GetAnnotation(ann);
            
            var z = x.Nodes.Add(colName + ": " + aT);
            z.ImageIndex = 3;
            z.SelectedImageIndex = 3;

            sb_ExportScript.Append("Model.Tables[\""+baseTbl+"\"].Columns[\""+colName+"\"].SetAnnotation(\""+annPrefix+aggNum+"\",\""+aT+"\")" + newline);
        }
        // foreach (var t in Model.Tables.Where(a => a.GetAnnotations().Any(b => b.StartsWith(annPrefix))).ToList())
        // {
        //     string tableName = t.Name;
            
        //     var x = step3TreeView.Nodes.Add(tableName);
        //     x.ImageIndex = 2;
        //     x.SelectedImageIndex = 2;                                

            // foreach (var a in t.GetAnnotations().Where(a => a.StartsWith(annPrefix)).ToList())
            // {
            //     int aggNumInd = a.LastIndexOf("_");
            //     int aggNum = Convert.ToInt32(a.Substring(aggNumInd+1));
            //     string aggTable = tableName + "_" + aggSuffix + "_" + aggNum;
            //     string ann = annPrefix+aggNum;
            //     string pr = t.GetAnnotation(ann);
                
            //     var y = x.Nodes.Add(aggTable + ": Precedence " + pr);
            //     y.ImageIndex = 2;
            //     y.SelectedImageIndex = 2;                

                // foreach (var c in t.Columns.Where(b => b.HasAnnotation(ann)).ToList())
                // {
                //     string colName = c.Name;
                //     string aT = c.GetAnnotation(ann);
                    
                //     var z = y.Nodes.Add(colName + ": " + aT);
                //     z.ImageIndex = 3;
                //     z.SelectedImageIndex = 3;
                // }
            //}    
       // }
    }
};

int goButtonX = 190;
int goButtonY = 330;
// Event Handlers (Start Screen)
newbatchButton.Click += (System.Object sender, System.EventArgs e) => {​

    goButton.Visible = true;
    goButton.Location = new Point(goButtonX, goButtonY);
    existingComboBox.Visible = false;
    goButton.Enabled = true;
    existingComboBox.Text = string.Empty;    
};

existingbatchButton.Click += (System.Object sender, System.EventArgs e) => {​

    goButton.Location = new Point(goButtonX, goButtonY+30);
    existingComboBox.Visible = true;
    goButton.Visible = true;    
    goButton.Enabled = false;
    
    // Populate Batch Combo Box
    existingComboBox.Items.Clear();

    foreach (var t in Model.Tables.Where(a => a.GetAnnotations().Any(b => b.StartsWith(annPrefix))).ToList())
	{
	    string tableName = t.Name;
	    
	    foreach (var a in t.GetAnnotations().Where(a => a.StartsWith(annPrefix)).ToList())
	    {
	        int aggNumInd = a.LastIndexOf("_");
	        int aggNum = Convert.ToInt32(a.Substring(aggNumInd+1));
	        string aggTable = tableName + "_" + aggSuffix + "_" + aggNum;
	        
	        existingComboBox.Items.Add(aggTable);
	    }
	}

    if (existingComboBox.SelectedItem == null)
    {
        goButton.Enabled = false;
    }  
};

existingComboBox.SelectedValueChanged += (System.Object sender, System.EventArgs e) => {​

    goButton.Enabled = true;         
};

goButton.Click += (System.Object sender, System.EventArgs e) => {​

    startPanel.Visible = false;
    topPanel.Visible = true;
    namePanel.Visible = true;
    aggPanel.Visible = true;
    infoPanel.Visible = true;
    treePanel.Visible = true;
    currentPage = 1;
    
    if (existingbatchButton.Checked == true)
    {
        aggTableName = existingComboBox.Text;
        baseTableName = aggTableName.Substring(0,aggTableName.IndexOf("_"+aggSuffix));
        aggNumber = Convert.ToInt32(aggTableName.Substring(aggTableName.LastIndexOf("_")+1)); 
        aggNameTextBox.Text = aggTableName;
        existingComboBox.Enabled = false;             
        baseNameComboBox.Enabled = false;
        baseNameComboBox.Text = baseTableName;
		prec = Convert.ToInt32(Model.Tables[baseTableName].GetAnnotation(annPrefix+aggNumber.ToString()));
        precNumericUpDown.Enabled = true;
        precNumericUpDown.Value = prec;
         
        NextStep(1); 
    }
    // else
    // {
    //     //NextStep(0); 
    // }
};

baseNameComboBox.SelectedValueChanged += (System.Object sender, System.EventArgs e) => {​

    precNumericUpDown.Enabled = true;
    precNumericUpDown.Value = 0;
    baseTableName = baseNameComboBox.Text;

    if (!Model.Tables[baseTableName].GetAnnotations().Any(a => a.StartsWith(annPrefix)))
    {
    	aggNameTextBox.Text = baseTableName + "_" + aggSuffix + "_" + "1";

        NextStep(0);
    }
    else if (existingbatchButton.Checked)
    {
        aggNameTextBox.Text = aggTableName;
    }
    else
    {
        // Update AggNameTextBox
    	int i=0;
		foreach (var a in Model.Tables[baseTableName].GetAnnotations().Where(a => a.StartsWith("TabularAggWizard_")))
		{
		    int b = Convert.ToInt32(a.Substring(a.IndexOf(annPrefix)+annPrefixLen));
		    if (b > i)
		    {
		        i=b;
		    }
		}

    	aggNameTextBox.Text = baseTableName + "_" + aggSuffix + "_" + (i+1).ToString();

        NextStep(0);
    }
};

step1TreeView.AfterExpand += (System.Object sender, System.Windows.Forms.TreeViewEventArgs e) => {​ 
    
    IsExpOrCol = true;
};

step1TreeView.AfterCollapse += (System.Object sender, System.Windows.Forms.TreeViewEventArgs e) => {​ 
    
    IsExpOrCol = true;
};

step1TreeView.NodeMouseClick += (System.Object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e) => {​ 

    IsExpOrCol = false;
};

// Top buttons
int saveButtonX = 330;
int buttonY = 25;
int buttonGap = 30;
int buttonSize = 25;

saveButton.Visible = true;
saveButton.ImageIndex = 0;
saveButton.Size = new Size(buttonSize,buttonSize);
saveButton.Location = new Point(saveButtonX,buttonY);
saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
saveButton.FlatAppearance.BorderSize = 0;
saveButton.TabStop = false;

deleteButton.Visible = true;
deleteButton.ImageIndex = 2;
deleteButton.Size = new Size(buttonSize,buttonSize);
deleteButton.Location = new Point(saveButtonX+(buttonGap*1),buttonY);
deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
deleteButton.FlatAppearance.BorderSize = 0;
deleteButton.TabStop = false;

scriptButton.Visible = false;
scriptButton.ImageIndex = 1;
scriptButton.Size = new Size(buttonSize,buttonSize);
scriptButton.Location = new Point(saveButtonX+(buttonGap*2),buttonY);
scriptButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
scriptButton.FlatAppearance.BorderSize = 0;
scriptButton.TabStop = false;

recoButton.Visible = true;
recoButton.ImageIndex = 11;
recoButton.Size = new Size(buttonSize,buttonSize);
recoButton.Location = new Point(saveButtonX+(buttonGap*2),buttonY);
recoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
recoButton.FlatAppearance.BorderSize = 0;
recoButton.TabStop = false;

backButton.Visible = true;
backButton.ImageIndex = 10;
backButton.Size = new Size(buttonSize,buttonSize);
backButton.Location = new Point(saveButtonX+(buttonGap*3),buttonY);
backButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
backButton.FlatAppearance.BorderSize = 0;
backButton.TabStop = false;

nextButton.Visible = true;
nextButton.ImageIndex = 3;
nextButton.Size = new Size(buttonSize,buttonSize);
nextButton.Location = new Point(saveButtonX+(buttonGap*4),buttonY);
nextButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
nextButton.FlatAppearance.BorderSize = 0;
nextButton.TabStop = false;

saveButton.MouseEnter += (System.Object sender, System.EventArgs e) => {
      
    //saveButton.ImageIndex = 0;
    saveButton.FlatAppearance.BorderSize = 0;
};

saveButton.MouseLeave += (System.Object sender, System.EventArgs e) => {
      
    //saveButton.ImageIndex = 0;
    saveButton.FlatAppearance.BorderSize = 0;
};

deleteButton.MouseEnter += (System.Object sender, System.EventArgs e) => {
      
    //deleteButton.ImageIndex = 2;
    deleteButton.FlatAppearance.BorderSize = 0;
};

deleteButton.MouseLeave += (System.Object sender, System.EventArgs e) => {
      
    //deleteButton.ImageIndex = 2;
    deleteButton.FlatAppearance.BorderSize = 0;
};

nextButton.MouseEnter += (System.Object sender, System.EventArgs e) => {
      
    //nextButton.ImageIndex = 3;
    nextButton.FlatAppearance.BorderSize = 0;
};

nextButton.MouseLeave += (System.Object sender, System.EventArgs e) => {
      
    //nextButton.ImageIndex = 3;
    nextButton.FlatAppearance.BorderSize = 0;
};

scriptButton.MouseEnter += (System.Object sender, System.EventArgs e) => {
      
    //scriptButton.ImageIndex = 1;
    scriptButton.FlatAppearance.BorderSize = 0;
};

scriptButton.MouseLeave += (System.Object sender, System.EventArgs e) => {
      
    //scriptButton.ImageIndex = 1;
    scriptButton.FlatAppearance.BorderSize = 0;
};

backButton.MouseEnter += (System.Object sender, System.EventArgs e) => {
      
    //backButton.ImageIndex = 4;
    backButton.FlatAppearance.BorderSize = 0;
};

backButton.MouseLeave += (System.Object sender, System.EventArgs e) => {
      
    //backButton.ImageIndex = 4;
    backButton.FlatAppearance.BorderSize = 0;
};

recoButton.MouseEnter += (System.Object sender, System.EventArgs e) => {
      
    //recoButton.ImageIndex = 5;
    recoButton.FlatAppearance.BorderSize = 0;
};

recoButton.MouseLeave += (System.Object sender, System.EventArgs e) => {
      
    //recoButton.ImageIndex = 5;
    recoButton.FlatAppearance.BorderSize = 0;
};

saveButton.Click += (System.Object sender, System.EventArgs e) => {​

    int rootNodeSelCount = 0;
    int rootNodeCount = step1TreeView.Nodes.Count;
    aggTableName = aggNameTextBox.Text;
    aggNumber = Convert.ToInt32(aggTableName.Substring(aggTableName.LastIndexOf("_")+1));
    foreach (System.Windows.Forms.TreeNode rootNode in step1TreeView.Nodes)
    {
        if (rootNode.Checked)
        {
            rootNodeSelCount++;
        }
    }

    if (currentPage == 1)
    {
    	// calculation for comparing 2 agg tables (if the same)
    	string erMes = string.Empty;
    	int[] md = new int[1000];
    	int z=0;
    	foreach (System.Windows.Forms.TreeNode rootNode in step1TreeView.Nodes)
        {
        	string cName = rootNode.Text;
        	int mdInd = Model.Tables[baseTableName].Columns[cName].MetadataIndex;

        	if (rootNode.Checked)
        	{
        		md[z] = mdInd;
        		z++;
        	}
        	
        }

        foreach (var an in Model.Tables[baseTableName].GetAnnotations().Where(a => a.StartsWith(annPrefix) && a != annPrefix+aggNumber).ToList())
        {
        	string aTName = baseTableName+"_"+aggSuffix+"_"+an.Substring(an.LastIndexOf("_")+1);
        	int[] mdCheck = new int[1000];
        	int y=0;
        	foreach (var c in Model.Tables[baseTableName].Columns.Where(a => a.HasAnnotation(an)).OrderBy(a => a.Name).ToList())
        	{
        		mdCheck[y] = c.MetadataIndex;
        		y++;
        	}
        	if (md.SequenceEqual(mdCheck))
        	{
        		erMes = "The columns selected match an existing agg table ('"+aTName+"). Agg tables within the same base table must have different columns.";
        	}
        }

        if (baseNameComboBox.Text == null)
        {
            Error("Please select a table to aggregate.");
            return;            
        }
        else if (rootNodeSelCount < 2)
        {
            Error("Please select at least 2 columns to aggregate.");
            return;
        }
        // agg columns selcted are the same as another agg table
        else if (erMes.Length > 0)
        {
        	Error(erMes);
        	return;
        }
        else
        {
            prec = Convert.ToInt32(precNumericUpDown.Value);
   
            // Update table annotation
            Model.Tables[baseTableName].SetAnnotation(annPrefix+aggNumber,prec.ToString());

            // Update column annotations
            foreach (System.Windows.Forms.TreeNode rootNode in step1TreeView.Nodes)
            {
                string colName = rootNode.Text;
                string ann = annPrefix+aggNumber;

                var c =  Model.Tables[baseTableName].Columns[colName];
                aggType = c.GetAnnotation(ann);
                
                if (aggType == null)
                {
                    aggType = "";
                }

                if (rootNode.Checked)
                {

                    c.SetAnnotation(ann,aggType);
                }
                else
                {
                	c.RemoveAnnotation(ann);
                }
            }

            hasSaved = true;
        }
    }
    else if (currentPage == 2)
    {
        int cbCount = 0;
        int cbFilledCount = 0;
        // Check if all boxes are filled
        foreach (System.Windows.Forms.Control cCB in s2treePanel.Controls.OfType<System.Windows.Forms.ComboBox>())
        {
            if (cCB.Text != "")
            {
                cbFilledCount+=1;
            }
            cbCount+=1;
        }

        if (cbFilledCount == cbCount)
        {
            foreach (System.Windows.Forms.Control cCB in s2treePanel.Controls.OfType<System.Windows.Forms.ComboBox>())
            {
                string columnName = cCB.Name;
                string ann = annPrefix+aggNumber;
                string aT = cCB.Text;
                var obj = Model.Tables[baseTableName].Columns[columnName];
                if (cCB.Text != string.Empty)
                {
                    obj.SetAnnotation(ann,aT);
                }
                else
                {
                    Error("Must fill summarizations for all columns in order to save.");
                    return;
                    //obj.RemoveAnnotation(ann);
                }

                // step11treeView.SelectedNode.ImageIndex = 9;
                // step11treeView.SelectedNode.SelectedImageIndex = 9;
            }
        }
        // Remove all annotations if none are filled
        // else if (cbFilledCount == 0)
        // {
        //     foreach (System.Windows.Forms.Control cCB in s2treePanel.Controls.OfType<System.Windows.Forms.ComboBox>())
        //     {
        //         string columnName = cCB.Name;
        //         string ann = perspName+"_AggType";
        //         var obj = Model.Tables[tableName].Columns[columnName];
        //         obj.RemoveAnnotation(ann);  

        //         step11treeView.SelectedNode.ImageIndex = 0;
        //         step11treeView.SelectedNode.SelectedImageIndex = 0;              
        //     }
        // }
        // If some are filled, raise an error
        else
        {
            Error("Must fill summarizations for all columns in order to save.");
        }
    }
};

deleteButton.Click += (System.Object sender, System.EventArgs e) => {​

    string ann = annPrefix+aggNumber;

    if (currentPage == 1 || currentPage == 2)
    {
        if (hasSaved == false && newbatchButton.Checked)
        {
            Error("No valid agg table to delete.");
            return;
        }
        else
        {
            if (System.Windows.Forms.MessageBox.Show("Are you sure you want to delete '"+aggTableName+"'?","Delete Agg Table",System.Windows.Forms.MessageBoxButtons.YesNo,System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                var tbl = Model.Tables[baseTableName];
                tbl.RemoveAnnotation(ann);

                foreach (var c in tbl.Columns.ToList())
                {
                    c.RemoveAnnotation(ann);
                }

                Info("Agg table '"+aggTableName+"' has been deleted.");
            }
        } 
    }   
};

nextButton.Click += (System.Object sender, System.EventArgs e) => {​

    if (currentPage == 1)
    {
        if (hasSaved == false && newbatchButton.Checked)
        {
            Error("Please save an agg table in order to move to the next step.");
            return;
        }
        else
        {
            currentPage = 2;
            namePanel.Visible = false;
            aggPanel.Visible = false;
            infoPanel.Visible = false;
            treePanel.Visible = false;
            s2namePanel.Visible = true;
            s2aggPanel.Visible = true;
            s2treePanel.Visible = true;
            saveButton.ImageIndex = 0;
            backButton.ImageIndex = 4;     
            recoButton.ImageIndex = 5;

            NextStep(2);
            s3aggComboBox.Items.Clear();
            foreach (var t in Model.Tables.Where(a => a.GetAnnotations().Any(b => b.StartsWith(annPrefix))).ToList())
            {
                string tableName = t.Name;
                
                foreach (var a in t.GetAnnotations().Where(a => a.StartsWith(annPrefix)).ToList())
                {
                    int aggNumInd = a.LastIndexOf("_");
                    int aggNum = Convert.ToInt32(a.Substring(aggNumInd+1));
                    string aT = tableName+"_"+aggSuffix+"_"+aggNum;
                    s3aggComboBox.Items.Add(aT);
                }
            }

        }
    }
    else if (currentPage == 2)
    {
        currentPage = 3;
        s2namePanel.Visible = false;
        s2aggPanel.Visible = false;
        s2treePanel.Visible = false;
        s3aggPanel.Visible = true;
        s3infoPanel.Visible = true;
        s3treePanel.Visible = true;
        recoButton.Visible = false;
        scriptButton.Visible = true;
        s3aggComboBox.Text = aggTableName;
        saveButton.ImageIndex = 6;
        backButton.ImageIndex = 4;
        deleteButton.ImageIndex = 8;
        nextButton.ImageIndex = 9;

        NextStep(3);
    }
};


scriptButton.Click += (System.Object sender, System.EventArgs e) => {​      

    // Save export script to desktop
    string desktopPath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
    System.IO.File.WriteAllText(desktopPath + @"\"+annPrefix+aggNameExport+".cs", sb_ExportScript.ToString());

    Info("A script to generate '"+aggNameExport+"' has been saved to the desktop.");
};

backButton.Click += (System.Object sender, System.EventArgs e) => {​      

    if (currentPage == 2)
    {
        currentPage = 1;
        s2namePanel.Visible = false;
        s2aggPanel.Visible = false;
        s2treePanel.Visible = false; 
        namePanel.Visible = true;
        aggPanel.Visible = true;
        infoPanel.Visible = true;
        treePanel.Visible = true;
        saveButton.ImageIndex = 0;
        recoButton.ImageIndex = 11;
        backButton.ImageIndex = 10;
    }
    else if (currentPage == 3)
    {
        currentPage = 2;
        s3aggPanel.Visible = false;
        s3infoPanel.Visible = false;
        s3treePanel.Visible = false;
        s2namePanel.Visible = true;
        s2aggPanel.Visible = true;
        s2treePanel.Visible = true;
        recoButton.Visible = true;
        scriptButton.Visible = false;
        saveButton.ImageIndex = 0;
        recoButton.ImageIndex = 5;
        deleteButton.ImageIndex = 2;    
        nextButton.ImageIndex = 3;  
    }
};

s3aggComboBox.SelectedValueChanged += (System.Object sender, System.EventArgs e) => {​

    NextStep(3);
};

recoButton.Click += (System.Object sender, System.EventArgs e) => {​      

    if (currentPage == 2)
    {        
        foreach (System.Windows.Forms.Control cL in s2treePanel.Controls.OfType<System.Windows.Forms.Label>())
        {
            string colName = cL.Text;
            var obj = Model.Tables[baseTableName].Columns[colName];
            string dt = obj.DataType.ToString();
            bool h = obj.IsHidden;
            int rCount = obj.UsedInRelationships.Count();
            string last2 = colName.Substring(colName.Length - 2);
            string last3 = colName.Substring(colName.Length - 3);
            foreach (System.Windows.Forms.Control cCB in s2treePanel.Controls.OfType<System.Windows.Forms.ComboBox>())
            {
                if (cCB.Name == colName)
                {
                    if (rCount > 0)
                    {
                        cCB.Text = "GroupBy";
                    }
                    else if (dt != "Decimal" && dt != "Double" && dt != "Int64")
                    {
                        cCB.Text = "GroupBy";
                    }
                    else if (h == false)
                    {
                        cCB.Text = "GroupBy";
                    }
                    else if (last2.ToUpper() == "ID" || last3.ToUpper() == "KEY")
                    {
                        cCB.Text = "GroupBy";
                    }
                    else
                    {
                        cCB.Text = "Sum";
                    }
                }
            }
        }
    }
};

ebiHome.LinkClicked += (System.Object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e) => {​      

System.Diagnostics.Process.Start(ebiURL);

};

ebiMain.LinkClicked += (System.Object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e) => {​      

System.Diagnostics.Process.Start(ebiURL);

};


// Dark Mode
if (darkModeEnabled)
{
	// foreach (System.Windows.Forms.Control c in treePanel.Controls.OfType<System.Windows.Forms.TreeView>())
// {
//     c.BackColor = Color.Black;
//    // c.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
// }

	// toolNameLabel.ForeColor = darkModeName;
	// topPanel.BackColor = darkModeBack;
	// namePanel.BackColor = darkModeBack;
	// typePanel.BackColor = darkModeBack;
	// infoPanel.BackColor = darkModeBack;
	// treePanel.BackColor = darkModeBack;
	// treeView.BackColor = darkModeBack;
	// summaryTreeView.BackColor = darkModeBack;
	// batchNameTextBox.BackColor = darkModeBack;
	// batchNameTextBox.ForeColor = darkModeText;
	// typeComboBox.BackColor = darkModeBack;
	// sequenceCheckBox.BackColor = darkModeBack;
	// summarySequenceCheckBox.BackColor = darkModeBack;
	// nameLabel.ForeColor = darkModeText;
	// typeLabel.ForeColor = darkModeText;
	// infoLabel.ForeColor = darkModeText;
	// sequenceCheckBox.ForeColor = darkModeText;
	// summarySequenceCheckBox.ForeColor = darkModeText;
	// maxPLabel.ForeColor = darkModeText;
	// maxPNumeric.BackColor = darkModeBack;
	// maxPNumeric.ForeColor = darkModeText;
	// summaryMaxPNumeric.BackColor = darkModeBack;
	// summaryMaxPNumeric.ForeColor = darkModeText;
	// typeComboBox.ForeColor = darkModeText;
	// summarytypeComboBox.ForeColor = darkModeText;
	// treeView.ForeColor = darkModeText;
	// summaryTreeView.ForeColor = darkModeText;
	// newForm.BackColor = darkModeBack;
	// newbatchButton.BackColor = darkModeBack;
	// newbatchButton.ForeColor = darkModeText;
	// existingbatchButton.BackColor = darkModeBack;
	// existingbatchButton.ForeColor = darkModeText;
	// batchComboBox.BackColor = darkModeBack;
	// batchComboBox.ForeColor = darkModeText;
	// goButton.ForeColor = darkModeText;
	//treeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
}

newForm.Show();