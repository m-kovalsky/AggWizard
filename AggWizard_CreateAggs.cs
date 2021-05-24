string annPrefix = "TabularAggWizard_";
string aggSuffix = "_AGG";
string[] aggTypeList = {"GroupBy","Sum","Count","Min","Max"};

// Error catching: Annotation values
foreach (var t in Model.Tables.Where(a => a.GetAnnotations().Any(b => b.StartsWith(annPrefix))).OrderBy(a => a.Name).ToList())
{
    string baseTableName = t.Name;
    
    foreach (var an in t.GetAnnotations().Where(a => a.StartsWith(annPrefix)).ToList())
    {
        if (t.GetAnnotation(an) == string.Empty)
        {
            Error("The '"+baseTableName+"' table has an invalid annotation value (precedence) for the '"+an+"' annotation.");
            return;
        }
        
        foreach (var c in t.Columns.Where(a => a.HasAnnotation(an)).ToList())
        {
            string colName = c.Name;
            if (!aggTypeList.Contains(c.GetAnnotation(an)))
            {
                Error("'"+baseTableName+"'["+colName+"] has an invalid annotation value (aggregation type) for the '"+an+"' annotation." );
                return;
            }
        }
    }

// Error catching: Partitions (legacy), SELECT*FROM
    foreach (var p in t.Partitions.OrderBy(a => a.Name).ToList())
    {
        string pName = p.Name;
        string q = p.Query.Replace(" ","").Replace("\n","").Replace("\r","").Replace("\t","").ToUpper();

        if (!p.ObjectTypeName.ToString().Contains("Legacy"))
        {
            Error("All partitions in the base table must be of type 'legacy'. '"+baseTableName+"'["+pName+"] is not a legacy partition.");
            return;
        }
        if (!q.StartsWith("SELECT*FROM"))
        {
            Error("All partitions in the base table must be in SELECT * FROM format. '"+baseTableName+"'["+pName+"] is not in this format.");
            return;
        }
    }
}

foreach (var t in Model.Tables.Where(a => a.GetAnnotations().Any(b => b.StartsWith(annPrefix))).OrderBy(a => a.Name).ToList())
{
    string baseTableName = t.Name;
    
    // non-descending: higher precedence considered first
    foreach (var an in t.GetAnnotations().Where(a => a.StartsWith(annPrefix)).OrderBy(a => Convert.ToInt32(t.GetAnnotation(a))).ToList())
    {
        int aggNumInd = an.LastIndexOf("_");
        int aggNum = Convert.ToInt32(an.Substring(aggNumInd+1));
        string aggTableName = baseTableName + aggSuffix + "_" + aggNum;
        string ann = annPrefix+aggNum;
        string pr = t.GetAnnotation(ann);
        
        // Create agg table
        var x = Model.AddTable(aggTableName);
        x.IsHidden = true;

        // Update agg annotations for batch processing
        foreach (var tableann in t.GetAnnotations().Where(a => a.StartsWith("TabularProcessingBatch_")).ToList())
        {
        	string annValue = Model.Tables[baseTableName].GetAnnotation(tableann);
        	x.SetAnnotation(tableann,annValue);
        }
        
        int pCount = Model.Tables[baseTableName].Partitions.Count();
        var dataSource = Model.Tables[baseTableName].Source;
        
        // Add/update partitions
        if (pCount > 1)
        {
            // Add partitions
            foreach(var p in Model.Tables[baseTableName].Partitions.ToList())
            {
                string aggPartitionName = p.Name.Replace(baseTableName,aggTableName);
                //string aggQuery = p.Query.Replace(baseTableName,aggTableName);
                var aggPar = Model.Tables[aggTableName].AddPartition(aggPartitionName);
                
                // Update Data Source
                aggPar.DataSource = Model.DataSources[dataSource];
                
                // Update Query
                aggPar.Query = p.Query;

                // Update agg annotations for batch processing
                foreach (var partann in p.GetAnnotations().Where(a => a.StartsWith("TabularProcessingBatch_")).ToList())
		        {
		        	string partannValue = Model.Tables[baseTableName].Partitions[p.Name].GetAnnotation(partann);
		        	aggPar.SetAnnotation(partann,partannValue);
		        }
            }
            
            // Remove default partition
            Model.Tables[aggTableName].Partitions[aggTableName].Delete();
        }        
        else
        {
            var par = Model.Tables[baseTableName].Partitions[0];
            var aggPar = Model.Tables[aggTableName].Partitions[0];
            // Update Data Source
            aggPar.DataSource = Model.DataSources[dataSource];
            
            // Update Query
            aggPar.Query = par.Query;

            // Update agg annotations for batch processing
            foreach (var partann in par.GetAnnotations().Where(a => a.StartsWith("TabularProcessingBatch_")).ToList())
	        {
	        	string partannValue = Model.Tables[baseTableName].Partitions[0].GetAnnotation(partann);
	        	aggPar.SetAnnotation(partann,partannValue);
	        }
        }
        
        // Add columns
        foreach (var c in Model.Tables[baseTableName].Columns.Where(a => a.HasAnnotation(ann)).ToList())
        {
            string columnName = c.Name;
            var y = x.AddDataColumn(columnName);
            y.SourceColumn = (Model.Tables[baseTableName].Columns[columnName] as DataColumn).SourceColumn;
            y.DataType = c.DataType;
            y.IsHidden = true;
            y.FormatString = c.FormatString;
            
            // Add Relationships
            foreach(var r in Model.Relationships.ToList().Where(a=> a.FromTable == Model.Tables[baseTableName] && a.FromColumn == Model.Tables[baseTableName].Columns[columnName]))
            {
                var addRel = Model.AddRelationship();
                addRel.FromColumn = Model.Tables[aggTableName].Columns[columnName];
                addRel.ToColumn = Model.Tables[r.ToTable.Name].Columns[r.ToColumn.Name];
                addRel.FromCardinality = r.FromCardinality;
                addRel.ToCardinality = r.ToCardinality;
                addRel.CrossFilteringBehavior = r.CrossFilteringBehavior;
                addRel.SecurityFilteringBehavior = r.SecurityFilteringBehavior;
                addRel.IsActive = r.IsActive;
                Model.Tables[aggTableName].Columns[columnName].SetAnnotation(aggTableName,"ForeignKey");
                Model.Tables[baseTableName].Columns[columnName].SetAnnotation(aggTableName,"ForeignKey");
            }
            
            // For non-key columns, create agg measures
            if ( Model.Tables[aggTableName].Columns[columnName].GetAnnotation(aggTableName) == null)
            {
                foreach (var z in Model.Tables[baseTableName].Columns[columnName].ReferencedBy.OfType<Measure>().ToList())
                {
                    string newMeasureName = z.Name + aggSuffix + aggNum;
                    string measureDAX = z.Expression;
                    var newDAX = measureDAX.Replace(baseTableName + "[" + columnName + "]",aggTableName + "[" + columnName + "]");
                    newDAX = newDAX.Replace("'" + baseTableName + "'" + "[" + columnName + "]","'" + aggTableName + "'" + "[" + columnName + "]");
                    string fs = z.FormatString;
                    string df = z.DisplayFolder;
                    var k = z.KPI;
                    string tbl = z.Table.Name;
                    
                    // Do not duplicate measures in case a single measure is referenced by multiple agg columns
                    if (! Model.AllMeasures.Any(a => a.Name == newMeasureName))
                    {
                        // Create agg measure, format same as non-agg measure
                        var newMeasure = Model.Tables[aggTableName].AddMeasure(newMeasureName);
                        newMeasure.Expression = newDAX; //FormatDax
                        newMeasure.IsHidden = true;
                        newMeasure.FormatString = fs;
                        newMeasure.DisplayFolder = df;
                        newMeasure.KPI = k;
                        Model.Tables[tbl].Measures[z.Name].SetAnnotation(aggTableName,"BaseMeasure");
                        
                        // Add new measures to respective perspectives
                        foreach (var p in Model.Perspectives.ToList())
                        {
                            foreach (var mea in Model.AllMeasures.Where(a=> a.Name == z.Name))
                            {
                                bool inPer = mea.InPerspective[p];
                                newMeasure.InPerspective[p] = inPer;
                                
                                // Set Annotations for base measures
                                mea.SetAnnotation(aggTableName,"BaseMeasure");
                            }
                        }
                        
                        // Set annotation denoting column as an agg column
                        Model.Tables[aggTableName].Columns[columnName].SetAnnotation(aggTableName,"AggColumn");
                        Model.Tables[baseTableName].Columns[columnName].SetAnnotation(aggTableName,"AggColumn");
                    }
                    // For multi-aggcolumn referenced measures
                    else
                    {
                    	var zz = Model.Tables[aggTableName].Measures[newMeasureName];
                    	string zexpr = zz.Expression;
                    	zexpr = zexpr.Replace(baseTableName + "[" + columnName + "]",aggTableName + "[" + columnName + "]");
                    	zexpr = zexpr.Replace("'" + baseTableName + "'" + "[" + columnName + "]","'" + aggTableName + "'" + "[" + columnName + "]");
                    	zz.Expression = zexpr;
                    }
                }
            }
        } 

        // Initialize DAX Statement string for Agg-check measure
        var sb = new System.Text.StringBuilder();
        sb.Append("IF (");
        
        // Create ISCROSSFILTERED Statement
        foreach (var c in Model.Tables[baseTableName].Columns.Where(b => b.GetAnnotation(aggTableName) != "ForeignKey" && b.GetAnnotation(aggTableName) != "AggColumn").ToList())
        {
            string columnName = c.Name;
            foreach(var r in Model.Relationships.ToList().Where(a=> a.FromTable == Model.Tables[baseTableName] && a.FromColumn == Model.Tables[baseTableName].Columns[columnName]))
            {
                string toTable = r.ToTable.Name;
                sb.Append("ISCROSSFILTERED('"+toTable+"') || ");
                Model.Tables[baseTableName].Columns[columnName].SetAnnotation(aggTableName,"ForeignKeyNotInAgg");
            }
        }   
        
        // Create ISFILTERED Statement    
        foreach (var c in Model.Tables[baseTableName].Columns.Where(b => b.GetAnnotation(aggTableName) == null).ToList())
        {
            string columnName = c.Name;
            sb.Append("ISFILTERED('"+baseTableName+"'["+columnName+"]) || ");
        }
        
        string dax = sb.ToString(0,sb.Length - 3) + ",0,1)";

        var m = Model.Tables[aggTableName].AddMeasure(baseTableName+"check"+aggNum);
        m.Expression = dax; //FormatDax
        m.IsHidden = true;
        
        // Add Agg-check measure to respective perspective(s)
        //foreach (var t in Model.Tables.Where (a => a.Name == baseTableName).ToList())
        //{        
        //    foreach (var p in Model.Perspectives.ToList())
        //    {
        //        bool inPersp = t.InPerspective[p];            
        //        m.InPerspective[p] = inPersp;
        //    }
        //}

        // Update non-agg measures to switch between agg & non-agg
        foreach (var o in Model.AllMeasures.Where(a => a.GetAnnotation(aggTableName) == "BaseMeasure").ToList())
        {
            string objName = o.Name;
            string expr = o.Expression;
            o.Expression = "IF([" + baseTableName + "check"+aggNum+"] = 1,[" + objName + aggSuffix + aggNum +"],"+expr+")"; //FormatDax
        } 
    } 
}

// Update Partition Queries
foreach (var t in Model.Tables.Where(a => a.GetAnnotations().Any(b => b.StartsWith(annPrefix))).OrderBy(a => a.Name).ToList())
{
    string baseTableName = t.Name;
    
    foreach (var an in t.GetAnnotations().Where(a => a.StartsWith(annPrefix)).ToList())
    {
        int aggNumInd = an.LastIndexOf("_");
        int aggNum = Convert.ToInt32(an.Substring(aggNumInd+1));
        string aggTableName = baseTableName + aggSuffix + "_" + aggNum;
        string ann = annPrefix+aggNum;
        string pr = t.GetAnnotation(ann);
        
        foreach (var p in Model.Tables[aggTableName].Partitions.ToList())
        {
            string q = p.Query;
            int fromInd = q.IndexOf("FROM");
            string sourceTable = q.Substring(fromInd+4).Trim();
            string newline = Environment.NewLine;
            string sql = string.Empty;
            
            var sb_Select = new System.Text.StringBuilder();
            string sb_From = ("FROM " + sourceTable + newline);
            var sb_GroupBy = new System.Text.StringBuilder();

            sb_Select.Append("SELECT" + newline);
            sb_GroupBy.Append("GROUP BY" + newline);
            
            int i=0;
            foreach (var c in Model.Tables[baseTableName].Columns.Where(a => a.HasAnnotation(ann)).OrderByDescending(a => a.UsedInRelationships.Where(b => b.FromTable.Name == baseTableName && b.FromColumn.Name == a.Name).Count()).ToList())
            {
                string colName = c.Name;
                string aT = c.GetAnnotation(ann);
                string sc = (Model.Tables[baseTableName].Columns[colName] as DataColumn).SourceColumn;
                if (i==0)
                {
                    if (aT == "GroupBy")
                    {
                        sb_Select.Append(" ["+sc+"]" + newline);
                        sb_GroupBy.Append(" ["+sc+"]" + newline);
                    }
                    else
                    {
                        sb_Select.Append(aT.ToUpper() + "( ["+sc+"]) AS ["+sc+"]" + newline);
                    }
                }
                else
                {
                    if (aT == "GroupBy")
                    {
                        sb_Select.Append(",["+sc+"]" + newline);
                        sb_GroupBy.Append(",["+sc+"]" + newline);
                    }
                    else
                    {
                        sb_Select.Append(","+aT.ToUpper() + "(["+sc+"]) AS ["+sc+"]" + newline);
                    }
                }
                
                i++;  
            }

            sql = sb_Select + sb_From + sb_GroupBy;
            p.Query = sql;                        
        }
    }
}


