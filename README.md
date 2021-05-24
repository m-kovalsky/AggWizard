# Agg Wizard

Agg Wizard is a tool that allows you to set customized aggregation tables for a tabular model. This tool runs inside of [Tabular Editor](https://tabulareditor.com/ "Tabular Editor")'s  [Advanced Scripting](https://docs.tabulareditor.com/Advanced-Scripting.html "Advanced Scripting") window and is compatible for all incarnations of tabular - [SQL Server Analysis Services](https://docs.microsoft.com/analysis-services/ssas-overview?view=asallproducts-allversions "SQL Server Analysis Services"), [Azure Analysis Services](https://azure.microsoft.com/services/analysis-services/ "Azure Analysis Services"), and [Power BI Premium](https://powerbi.microsoft.com/power-bi-premium/ "Power BI Premium") (using the [XMLA R/W
endpoint](https://docs.microsoft.com/power-bi/admin/service-premium-connect-tools "XMLA R/W Endpoint")). This aggregations built using this tool are also compatible irrespective of if the base fact table is in Direct Query or Import mode.

## Purpose

This tool is designed to automate the creation of aggregation tables and provide flexibility for Import models to take advantage of the performance benefits of aggregations.

## Running the Tool

To use the tool, download the AggWizard.cs script, paste it into the [Advanced Scripting](https://docs.tabulareditor.com/Advanced-Scripting.html "Advanced Scripting") window in [Tabular Editor](https://tabulareditor.com/ "Tabular Editor") and click the play button (or press F5).  The tool itself does not create the aggregation tables. It simply saves the instructions for creating the aggregation tables as metadata (annotations) within the model.

Make sure to click the 'Save' button within the Agg Wizard tool after making changes. If the 'Save' button is not clicked your changes will not be saved back to the model.

*Note: For easier access, it is recommended to save the script as a [Custom Action](https://docs.tabulareditor.com/Custom-Actions.html "Custom Action").*

## Creating the Agg Tables

After you use the Agg Wizard tool to customize your aggregation tables, you can use the following method to deploy your model to a server while simultaneously generating the aggregation tables.

1. Download the AggWizard_CreateAggs.cs script and save it to your computer.
2. Run the code below in the command prompt (filling in the \<parameters\>) according to the variety of tabular you are using.
    
***Note that although it is possible to run this in the command prompt, it is recommended to run the following processing scripts by integrating them into an application as discussed [below](https://github.com/m-kovalsky/ProcessingManager#integration-applications "Integration Applications").***

## [SQL Server Analysis Services](https://docs.microsoft.com/analysis-services/ssas-overview?view=asallproducts-allversions "SQL Server Analysis Services")

    start /wait /d "C:\Program Files (x86)\Tabular Editor" TabularEditor.exe "C:\Desktop\Model.bim" -D "<Server Name>" "<Database Name>" -S "<C# Script File Location (AggWizard_CreateAggs.cs)>"

## [Azure Analysis Services](https://azure.microsoft.com/services/analysis-services/ "Azure Analysis Services")

    start /wait /d "C:\Program Files (x86)\Tabular Editor" TabularEditor.exe "C:\Desktop\Model.bim" -D "Provider=MSOLAP;Data Source=asazure://<AAS Region>.asazure.windows.net/<AAS Server Name>;User ID=<xxxxx>;Password=<xxxxx>;Persist Security Info=True;Impersonation Level=Impersonate" "<Database Name>" -S "<C# Script File Location (AggWizard_CreateAggs.cs)>"

## [Power BI Premium](https://powerbi.microsoft.com/power-bi-premium/ "Power BI Premium")

Running this in Power BI Premium requires enabling [XMLA R/W endpoints](https://docs.microsoft.com/power-bi/admin/service-premium-connect-tools "XMLA R/W Endpoints") for your Premium Workspace. An additional requirement is setting up a [Service Principal](https://tabulareditor.com/2020/06/02/PBI-SP-Access.html "Setting up a Service Principal").

    start /wait /d "C:\Program Files (x86)\Tabular Editor" TabularEditor.exe "C:\Desktop\Model.bim" -D "Provider=MSOLAP;Data Source=powerbi://api.powerbi.com/v1.0/myorg/<Premium Workspace>;User ID=app:<Application ID>@<Tenant ID>;Password=<Application Secret>" "<Premium Dataset>" -S "<C# Script File Location (AggWizard_CreateAggs.cs)>" 


## Requirements

[Tabular Editor](https://tabulareditor.com/ "Tabular Editor") version 2.12.1 or higher.
