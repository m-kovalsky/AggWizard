# [Agg Wizard](https://www.elegantbi.com/post/aggwizard "Agg Wizard")

Agg Wizard is a tool that allows you to set customized aggregation tables for a tabular model. This tool runs inside of [Tabular Editor](https://tabulareditor.com/ "Tabular Editor")'s  [Advanced Scripting](https://docs.tabulareditor.com/Advanced-Scripting.html "Advanced Scripting") window and is compatible for all incarnations of tabular - [SQL Server Analysis Services](https://docs.microsoft.com/analysis-services/ssas-overview?view=asallproducts-allversions "SQL Server Analysis Services"), [Azure Analysis Services](https://azure.microsoft.com/services/analysis-services/ "Azure Analysis Services"), and [Power BI Premium](https://powerbi.microsoft.com/power-bi-premium/ "Power BI Premium") (using the [XMLA R/W
endpoint](https://docs.microsoft.com/power-bi/admin/service-premium-connect-tools "XMLA R/W Endpoint")). The aggregations built using this tool are also compatible irrespective of whether the base fact table is in Direct Query or Import mode.

## Purpose

This tool is designed to automate the creation of aggregation tables and provide flexibility for Import models to take advantage of the performance benefits of aggregations.

## Running the Tool

To use the tool, download the AggWizard.cs script, paste it into the [Advanced Scripting](https://docs.tabulareditor.com/Advanced-Scripting.html "Advanced Scripting") window in [Tabular Editor](https://tabulareditor.com/ "Tabular Editor") and click the play button (or press F5).  The tool itself does not create the aggregation tables. It simply saves the instructions for creating the aggregation tables as metadata (annotations) within the model.

Make sure to click the 'Save' button within the Agg Wizard tool after making changes. If the 'Save' button is not clicked your changes will not be saved back to the model.

*Note: For easier access, it is recommended to save the script as a [Custom Action](https://docs.tabulareditor.com/Custom-Actions.html "Custom Action").*

<img width="265" alt="Image1" src="https://user-images.githubusercontent.com/29556918/119328097-6ae88a00-bc8c-11eb-99d3-725e730cdaf7.png">

## Creating the Agg Tables

After you use the Agg Wizard tool to customize your aggregation tables, you can use the following method to deploy your model to a server while simultaneously generating the aggregation tables. It should be noted that the script mentioned below can also be run inside of Tabular Editor. However, I recommend only running the script in Tabular Editor for testing purposes (and not saving the changes).

1. Download the AggWizard_CreateAggs.cs script and save it to your computer.
2. Run the code below in the command prompt (filling in the \<parameters\>) according to the variety of tabular you are using.
    
***Note that although it is possible to run this in the command prompt, it is recommended to run the following processing scripts by integrating them into an application as discussed [below](https://github.com/m-kovalsky/ProcessingManager#integration-applications "Integration Applications").***

## [SQL Server Analysis Services](https://docs.microsoft.com/analysis-services/ssas-overview?view=asallproducts-allversions "SQL Server Analysis Services")

    start /wait /d "C:\Program Files (x86)\Tabular Editor" TabularEditor.exe "<C:\Desktop\Model.bim>" -D "<Server Name>" "<Database Name>" -S "<C# Script File Location (AggWizard_CreateAggs.cs)>"

## [Azure Analysis Services](https://azure.microsoft.com/services/analysis-services/ "Azure Analysis Services")

    start /wait /d "C:\Program Files (x86)\Tabular Editor" TabularEditor.exe "<C:\Desktop\Model.bim>" -D "Provider=MSOLAP;Data Source=asazure://<AAS Region>.asazure.windows.net/<AAS Server Name>;User ID=<xxxxx>;Password=<xxxxx>;Persist Security Info=True;Impersonation Level=Impersonate" "<Database Name>" -S "<C# Script File Location (AggWizard_CreateAggs.cs)>"

## [Power BI Premium](https://powerbi.microsoft.com/power-bi-premium/ "Power BI Premium")

Running this in Power BI Premium requires enabling [XMLA R/W endpoints](https://docs.microsoft.com/power-bi/admin/service-premium-connect-tools "XMLA R/W Endpoints") for your Premium Workspace. An additional requirement is setting up a [Service Principal](https://tabulareditor.com/service-principal-access-to-dedicated-capacity-xmla-endpoint/ "Setting up a Service Principal").

    start /wait /d "C:\Program Files (x86)\Tabular Editor" TabularEditor.exe "<C:\Desktop\Model.bim>" -D "Provider=MSOLAP;Data Source=powerbi://api.powerbi.com/v1.0/myorg/<Premium Workspace>;User ID=app:<Application ID>@<Tenant ID>;Password=<Application Secret>" "<Premium Dataset>" -S "<C# Script File Location (AggWizard_CreateAggs.cs)>" 

## Limitations

The Agg Wizard tool is compatible with all incarnations of tabular as well as with base fact tables in Direct Query or Import mode. That being said there are several limitations which are noted below.

*  Aggregations may only be created on fact tables (defined as having at least one relationship where it is on the 'from' side).
*  The partitions of the base fact table must be of '[provider-type](https://docs.microsoft.com/en-us/azure/analysis-services/analysis-services-datasource#understanding-providers)' (not using M and not a calculated table).
*  The partitions of the base fact table must all be in the format of 'SELECT * FROM ...'.
*  Only foreign keys and hidden 'aggregatable columns' can be used in the aggregation tables. Degenerate fact columns cannot be used in the aggregation table (and will not show in the Agg Wizard).

## Processing the Agg Tables

The aggregation tables must be processed as any other Import mode table would be processed. That being said, the Agg Wizard is integrated with the [Processing Manager](https://github.com/m-kovalsky/ProcessingManager "Processing Manager"). This means that if your model is using batch processing with the Processing Manager, the Agg tables (and appropriate partitions) will automatically be added to the same batch(es) as the base fact table. This is an easy way to ensure that your Agg tables are always up to date.

## Integration Applications

The command line code may be integrated into any application which is able to run command line code. Examples of such applications include [Azure DevOps](https://azure.microsoft.com/services/devops/ "Azure DevOps") and [Azure Data Factory](https://azure.microsoft.com/services/data-factory/ "Azure Data Factory"). Integrating the Processing Manager solution into these applciations will streamline the processing operations of your tabular model(s). In order to use these applications for a Power BI Premium dataset you will need to set up a [Service Principal](https://tabulareditor.com/2020/06/02/PBI-SP-Access.html "Service Principal") and a [Key Vault](https://azure.microsoft.com/services/key-vault/ "Azure Key Vault"). 

Make sure to read my [blog post](https://www.elegantbi.com/post/processingmanager "Processing Manager") for more information on configuring Tabular Editor scripts to run within [Azure DevOps](https://azure.microsoft.com/services/devops/ "Azure DevOps"). 

## Requirements

[Tabular Editor](https://tabulareditor.com/ "Tabular Editor") version 2.12.1 or higher.
