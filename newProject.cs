﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

// para arraylist
using System.Collections;

using System.Threading;


using System.Media;

namespace myWay
{
    public partial class newProject : Form
    {

        public project pr;

        public newProject()
        {
            InitializeComponent();

            // fill the combobox with enum...

            cmbDataType.DataSource = Enum.GetValues(typeof(project.databaseType));


            if (pr != null)
            {
                txtName.Text = pr.name;
                txtHost.Text = pr.host;
                txtDatabase.Text = pr.database;
                txtUser.Text = pr.user;
                txtPassword.Text = pr.password;
                cmbDataType.SelectedItem = pr.dbDataType.ToString();

                
            }

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

       

        
 

        private void search(object file)
        {
            bool right = false;
            string errorMessage = "";
            AsyncEnableButton(false);
            try
            {
                project pro = new project();
                pro = (project)file;

                Cursor.Current = Cursors.WaitCursor;

                switch (pro.dbDataType)
                {
                    case project.databaseType.mySql:
                        String connectionString = null;                        


                        connectionString = "Server=" + pr.host + ";Database=" + pr.database + ";Uid=" + pr.user + ";Pwd=" + pr.password + ";";



                        dbMySql db = new dbMySql();
                        errorMessage = db.test(connectionString);
                        if (errorMessage.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            //pr = new project();
                            //pr.name = pr.name;

                            // lets get the tables...
                            List<table> lista = new List<table>();
                            lista = db.getTables(connectionString, pr.database);
                            //lista.Sort();
                            pr.tables.Clear();
                            foreach (table item in lista)
                            {
                                AsyncWriteLine("Found table... " + item.Name + "\n");

                                // now lets get the fields for each table...
                                List<field> listaField = new List<field>();
                                listaField = db.getFields(connectionString, pr.database, item.Name);
                                

                                if (listaField != null)
                                {
                                    foreach (field fi in listaField)
                                    {
                                        item.fields.Add(fi);
                                        AsyncWriteLine("Found field... " + fi.Name + "\n");
                                    }


                                    // the descriptionField its the first string field of table...
                                    foreach (field campito in listaField)
                                    {
                                        if (campito.type.ToString().Equals("_string") || campito.type.ToString().Equals("_text"))
                                        {
                                            item.fieldDescription = campito.Name;
                                            break;
                                        }
                                    }
                                    if (item.fieldDescription == null)
                                        item.fieldDescription = listaField[0].Name;
                                }

                                // lets get primary keys and foreign keys for the table...
                                db.getKeys(connectionString, item, pr.database);


                                // lets sort the fields in the table...
                                // we order but put first key fields
                                if (general.orderFields)
                                {
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.name));
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.key));
                                }

                                pr.tables.Add(item);


                            }

                            pr.tables.Sort();


                            //// now lets get the relations ...
                            pr.relations.Clear();
                            List<relation> listarelation = new List<relation>();
                            listarelation = db.getRelations(connectionString, pr.database);
                            if (listarelation != null)
                            {

                                foreach (relation re in listarelation)
                                {
                                    // found description of fields...
                                    foreach (table item in pr.tables)
                                    {
                                        if (item.Name.ToLower().Equals(re.childTable.ToLower()))
                                        {
                                            re.childDescription = item.fieldDescription;
                                            // we put the field as keyfield...
                                            foreach (field fi in item.fields)
                                            {
                                                if (fi.Name.ToLower().Equals(re.childField.ToLower()))
                                                    fi.isForeignKey = true;
                                            }
                                        }

                                        if (item.Name.ToLower().Equals(re.parentTable.ToLower()))
                                            re.parentDescription = item.fieldDescription;
                                    }

                                    if (!pr.existsRelation(re.parentTable, re.childTable))
                                    {
                                        pr.relations.Add(re);
                                        AsyncWriteLine("Found relation... " + re.name + "\n");
                                    }

                                    // now if the relation has to do with the tables...
                                    foreach (table item in pr.tables)
                                    {
                                        // we put the relation in the child table...
                                        if (item.Name.ToLower().Equals(re.childTable.ToLower()))
                                            item.relations.Add(re);
                                    }

                                }

                            }


                            // also we can get relations about the field names
                            foreach (table tab in pr.tables)
                            {
                                foreach (field campo in tab.fields)
                                {
                                    if (campo.isKey)
                                    {
                                        foreach (table tab2 in pr.tables)
                                        {
                                            if (!tab.Name.ToLower().Equals(tab2.Name.ToLower()))
                                            {
                                                foreach (field campo2 in tab2.fields)
                                                {
                                                    if (campo.Name.ToLower().Equals(campo2.Name.ToLower()))
                                                    {
                                                        // check if relation exists..
                                                        if (!pr.existsRelation(tab.Name, tab2.Name))
                                                        {
                                                            campo2.isForeignKey = true;
                                                            relation rel = new relation();
                                                            rel.name = tab2.Name + "_" + tab.Name;

                                                            bool found = false;
                                                            foreach (relation relax in pr.relations)
                                                            {
                                                                if (relax.name.Equals(rel.name))
                                                                    found = true;
                                                            }
                                                            if (!found)
                                                            {
                                                                rel.parentTable = tab.Name;
                                                                rel.parentField = campo.Name;

                                                                rel.childTable = tab2.Name;
                                                                rel.childField = campo2.Name;

                                                                // found description of fields...
                                                                foreach (table item in pr.tables)
                                                                {
                                                                    if (item.Name.ToLower().Equals(rel.childTable.ToLower()))
                                                                        rel.childDescription = item.fieldDescription;

                                                                    if (item.Name.ToLower().Equals(rel.parentTable.ToLower()))
                                                                        rel.parentDescription = item.fieldDescription;
                                                                }

                                                                pr.relations.Add(rel);

                                                                // now if the relation has to do with the tables...
                                                                foreach (table item in pr.tables)
                                                                {
                                                                    if (item.Name.Equals(tab2.Name))
                                                                        item.relations.Add(rel);
                                                                }
                                                            }



                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            right = true;
                            pr.host = pro.host;
                            pr.database = pro.database;
                            pr.user = pro.user;
                            pr.password = pro.password;
                            pr.dbDataType = pro.dbDataType;

                            //pr.saveProject(Path.Combine(util.projects_dir, pro.name + ".xml"));

                            // lets save it for next load of application...
                            // pr.saveProject(Path.Combine(util.projects_dir, "conf.xml"));
                            //  AsyncWriteLine("Project saved... \n");



                        }
                        else
                        {
                            AsyncWrite(errorMessage);
                        }
                        break;

                    case project.databaseType.SqlServer:
                       
                        
 
                        connectionString = "Data Source=" + pro.host + ";Network Library=DBMSSOCN;Initial Catalog=" + pro.database + ";User ID=" + pro.user + ";Password=" + pro.password + ";";
                        // connectionStringOleDb = "Provider=SQLNCLI;Server=" + txtHost.Text + ";Database=" + txtDatabase.Text + ";Uid=" + txtUser.Text + ";Pwd=" + txtPassword.Text + ";";



                        dbSql2005 dbSqlServer = new dbSql2005();
                        errorMessage = dbSqlServer.test(connectionString);
                        if (errorMessage.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            //pr = new project();
                            //pr.name = pro.name;

                            // lets get the tables...
                            List<table> lista = new List<table>();
                            lista = dbSqlServer.getTables(connectionString, pro.database);
                            //lista.Sort();
                            foreach (table item in lista)
                            {
                                AsyncWriteLine("Found table... " + item.Name + "\n");

                                // now lets get the fields for each table...
                                List<field> listaField = new List<field>();
                                listaField = dbSqlServer.getFields(connectionString, item.Name);
                                if (listaField != null)
                                {
                                    foreach (field fi in listaField)
                                    {
                                        item.fields.Add(fi);
                                        AsyncWriteLine("Found field... " + fi.Name + "\n");
                                    }

                                    // the descriptionField its the first string field of table...
                                    foreach (field campito in listaField)
                                    {
                                        if (campito.type.ToString().Equals("_string") || campito.type.ToString().Equals("_text"))
                                        {
                                            item.fieldDescription = campito.Name;
                                            break;
                                        }
                                    }
                                    if (item.fieldDescription == null)
                                        item.fieldDescription = listaField[0].Name;

                                }

                                // lets get primary keys and foreign keys for the table...
                                dbSqlServer.getKeys(connectionString, item);
                                // lets get not primary keys
                                foreach (field campito in item.fields)
                                {
                                    if (!campito.isKey)
                                        item.getNotKeyFields.Add(campito);
                                }


                                // lets sort the fields in the table...
                                // we order but put first key fields
                                if (general.orderFields)
                                {
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.name));
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.key));
                                }
                                pr.tables.Add(item);


                                // lets get description of table
                                string DescriptionOfTable = "";
                                DescriptionOfTable = dbSqlServer.getCommentsFromTable(connectionString, item.Name);
                                if (DescriptionOfTable.IndexOf("#exclude#") != -1)
                                {
                                    item.excludeFromGeneration = true;
                                    DescriptionOfTable.Replace("#exclude#", "");
                                }
                                if (!DescriptionOfTable.Equals(""))
                                    item.TargetName = DescriptionOfTable;

                                // end of description for table


                            }

                            pr.tables.Sort();
                            // now lets get the relations ...
                            List<relation> listarelation = new List<relation>();
                            listarelation = dbSqlServer.getRelations(connectionString);
                            if (listarelation != null)
                            {
                                foreach (relation re in listarelation)
                                {
                                    //  item.fields.Add(re);
                                    pr.relations.Add(re);
                                    AsyncWriteLine("Found relation... " + re.name + "\n");

                                    // now if the relation has to do with the tables...
                                    foreach (table item in pr.tables)
                                    {
                                        // we put the relation in the parent table...
                                        if (item.Name.ToLower().Equals(re.parentTable.ToLower()))
                                        {
                                            // le añadimos la descripcion
                                            re.parentDescription = item.fieldDescription;

                                            foreach (table taby in pr.tables)
                                            {
                                                if (taby.Name.Equals(re.childTable))
                                                    re.childDescription = taby.fieldDescription;
                                            }
                                            item.relations.Add(re);
                                        }
                                    }


                                }

                            }


                            // also we can get relations about the field names
                            foreach (table tab in pr.tables)
                            {
                                foreach (field campo in tab.fields)
                                {
                                    if (campo.isKey)
                                    {
                                        foreach (table tab2 in pr.tables)
                                        {
                                            if (!tab.Name.ToLower().Equals(tab2.Name.ToLower()))
                                            {
                                                foreach (field campo2 in tab2.fields)
                                                {
                                                    if (campo.Name.ToLower().Equals(campo2.Name.ToLower()))
                                                    {
                                                        campo2.isForeignKey = true;
                                                        relation rel = new relation();
                                                        rel.name = tab.Name + "_" + tab2.Name;
                                                        if (!pr.relations.Contains(rel.name))
                                                        {
                                                            rel.parentTable = tab2.Name;
                                                            rel.parentField = campo2.Name;

                                                            rel.childTable = tab.Name;
                                                            rel.childField = campo.Name;

                                                            // found description of fields...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.ToLower().Equals(rel.childTable.ToLower()))
                                                                    rel.childDescription = item.fieldDescription;

                                                                if (item.Name.ToLower().Equals(rel.parentTable.ToLower()))
                                                                    rel.parentDescription = item.fieldDescription;
                                                            }

                                                            pr.relations.Add(rel);

                                                            // now if the relation has to do with the tables...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.Equals(tab2.Name))
                                                                {
                                                                    // see if the relation exists..
                                                                    bool seguir = true;
                                                                    foreach (relation rel2 in tab2.relations)
                                                                    {
                                                                        if (rel2.name.ToLower().Equals(rel.name.ToLower()))
                                                                            seguir = false;
                                                                    }
                                                                    if (seguir)
                                                                        item.relations.Add(rel);
                                                                }

                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            right = true;
                            pr.host = pro.host;
                            pr.database = pro.database;
                            pr.user = pro.user;
                            pr.password = pro.password;
                            pr.dbDataType = pro.dbDataType;

                            //pr.saveProject(Path.Combine(util.projects_dir, pro.name + ".xml"));

                            // lets save it for next load of application...
                            // pr.saveProject(Path.Combine(util.projects_dir, "conf.xml"));
                            //AsyncWriteLine("Project saved... \n");



                        }
                        else
                        {
                            AsyncWrite(errorMessage);
                        }

                        break;

                    case project.databaseType.dbf:
                           

                           connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pro.database + ";Extended Properties=dBASE IV;User ID=" + pro.user + ";Password=" + pro.password + ";";
                                              
                        dbDbf dbf = new dbDbf();
                        errorMessage = dbf.test(connectionString);
                        if (errorMessage.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            //pr = new project();
                            //pr.name = pro.name;

                            // lets get the tables...
                            List<table> lista = new List<table>();
                            lista = dbf.getTables(connectionString, pro.database);
                            //lista.Sort();
                            foreach (table item in lista)
                            {
                                AsyncWriteLine("Found table... " + item.Name + "\n");

                                // now lets get the fields for each table...
                                List<field> listaField = new List<field>();
                                listaField = dbf.getFields(connectionString, item.Name);
                                if (listaField != null)
                                {
                                    foreach (field fi in listaField)
                                    {
                                        item.fields.Add(fi);
                                        AsyncWriteLine("Found field... " + fi.Name + "\n");
                                    }

                                    // the descriptionField its the first string field of table...
                                    foreach (field campito in listaField)
                                    {
                                        if (campito.type.ToString().Equals("_string") || campito.type.ToString().Equals("_text"))
                                        {
                                            item.fieldDescription = campito.Name;
                                            break;
                                        }
                                    }
                                    if (item.fieldDescription == null)
                                        item.fieldDescription = listaField[0].Name;

                                }

                                // lets get primary keys and foreign keys for the table...
                                dbf.getKeys(connectionString, item);

                                // lets sort the fields in the table...
                                // we order but put first key fields
                                if (general.orderFields)
                                {
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.name));
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.key));
                                }
                                pr.tables.Add(item);


                            }

                            pr.tables.Sort();
                            // now lets get the relations ...
                            List<relation> listarelation = new List<relation>();
                            listarelation = dbf.getRelations(connectionString);
                            if (listarelation != null)
                            {
                                foreach (relation re in listarelation)
                                {
                                    //  item.fields.Add(re);
                                    pr.relations.Add(re);
                                    AsyncWriteLine("Found relation... " + re.name + "\n");

                                    // now if the relation has to do with the tables...
                                    foreach (table item in pr.tables)
                                    {
                                        // we put the relation in the parent table...
                                        if (item.Name.ToLower().Equals(re.parentTable.ToLower()))
                                        {
                                            // le añadimos la descripcion
                                            re.parentDescription = item.fieldDescription;

                                            foreach (table taby in pr.tables)
                                            {
                                                if (taby.Name.Equals(re.childTable))
                                                    re.childDescription = taby.fieldDescription;
                                            }
                                            item.relations.Add(re);
                                        }
                                    }


                                }

                            }


                            // also we can get relations about the field names
                            foreach (table tab in pr.tables)
                            {
                                foreach (field campo in tab.fields)
                                {
                                    if (campo.isKey)
                                    {
                                        foreach (table tab2 in pr.tables)
                                        {
                                            if (!tab.Name.ToLower().Equals(tab2.Name.ToLower()))
                                            {
                                                foreach (field campo2 in tab2.fields)
                                                {
                                                    if (campo.Name.ToLower().Equals(campo2.Name.ToLower()))
                                                    {
                                                        campo2.isForeignKey = true;
                                                        relation rel = new relation();
                                                        rel.name = tab.Name + "_" + tab2.Name;
                                                        if (!pr.relations.Contains(rel.name))
                                                        {
                                                            rel.parentTable = tab2.Name;
                                                            rel.parentField = campo2.Name;

                                                            rel.childTable = tab.Name;
                                                            rel.childField = campo.Name;

                                                            // found description of fields...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.ToLower().Equals(rel.childTable.ToLower()))
                                                                    rel.childDescription = item.fieldDescription;

                                                                if (item.Name.ToLower().Equals(rel.parentTable.ToLower()))
                                                                    rel.parentDescription = item.fieldDescription;
                                                            }

                                                            pr.relations.Add(rel);

                                                            // now if the relation has to do with the tables...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.Equals(tab2.Name))
                                                                {
                                                                    // see if the relation exists..
                                                                    bool seguir = true;
                                                                    foreach (relation rel2 in tab2.relations)
                                                                    {
                                                                        if (rel2.name.ToLower().Equals(rel.name.ToLower()))
                                                                            seguir = false;
                                                                    }
                                                                    if (seguir)
                                                                        item.relations.Add(rel);
                                                                }

                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            right = true;
                            pr.host = pro.host;
                            pr.database = pro.database;
                            pr.user = pro.user;
                            pr.password = pro.password;
                            pr.dbDataType = pro.dbDataType;

                        }
                        else
                        {
                            AsyncWriteLine(errorMessage);
                        }
                        break;

                    case project.databaseType.access2003:


                        connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pro.database + ";User ID=" + pro.user + ";Password=" + pro.password + ";";

                        dbAccess dba2003 = new dbAccess();
                        errorMessage = dba2003.test(connectionString);
                        if (errorMessage.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            //pr = new project();
                            //pr.name = pro.name;

                            // lets get the tables...
                            List<table> lista = new List<table>();
                            lista = dba2003.getTables(connectionString, pro.database);
                            //lista.Sort();
                            foreach (table item in lista)
                            {
                                AsyncWriteLine("Found table... " + item.Name + "\n");

                                // now lets get the fields for each table...
                                List<field> listaField = new List<field>();
                                listaField = dba2003.getFields(connectionString, item.Name);
                                if (listaField != null)
                                {
                                    foreach (field fi in listaField)
                                    {
                                        item.fields.Add(fi);
                                        AsyncWriteLine("Found field... " + fi.Name + "\n");

                                    }

                                }

                                // lets get primary keys and foreign keys for the table...
                                dba2003.getKeys(connectionString, item);

                                // now we search a text field that is not key
                                if (listaField != null)
                                {
                                    // the descriptionField its the first string field of table...
                                    foreach (field campito in listaField)
                                    {
                                        if (campito.type.ToString().Equals("_string") && !campito.isKey)
                                        {
                                            item.fieldDescription = campito.Name;
                                            break;
                                        }

                                    }

                                }
                                if (item.fieldDescription == null)
                                    item.fieldDescription = item.GetKey;

                                // lets sort the fields in the table...
                                // we order but put first key fields
                                if (general.orderFields)
                                {
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.name));
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.key));
                                }
                                pr.tables.Add(item);


                            }

                            pr.tables.Sort();
                            // now lets get the relations ...
                            List<relation> listarelation = new List<relation>();
                            listarelation = dba2003.getRelations(connectionString);
                            if (listarelation != null)
                            {
                                foreach (relation re in listarelation)
                                {
                                    //  item.fields.Add(re);
                                    pr.relations.Add(re);
                                    AsyncWriteLine("Found relation... " + re.name + "\n");

                                    // now if the relation has to do with the tables...
                                    foreach (table item in pr.tables)
                                    {
                                        // we put the relation in the parent table...
                                        if (item.Name.ToLower().Equals(re.parentTable.ToLower()))
                                        {
                                            // le añadimos la descripcion
                                            re.parentDescription = item.fieldDescription;

                                            foreach (table taby in pr.tables)
                                            {
                                                if (taby.Name.Equals(re.childTable))
                                                    re.childDescription = taby.fieldDescription;
                                            }
                                            item.relations.Add(re);
                                        }
                                    }


                                }

                            }


                            // also we can get relations about the field names
                            foreach (table tab in pr.tables)
                            {
                                foreach (field campo in tab.fields)
                                {
                                    if (campo.isKey)
                                    {
                                        foreach (table tab2 in pr.tables)
                                        {
                                            if (!tab.Name.ToLower().Equals(tab2.Name.ToLower()))
                                            {
                                                foreach (field campo2 in tab2.fields)
                                                {
                                                    if (campo.Name.ToLower().Equals(campo2.Name.ToLower()))
                                                    {
                                                        campo2.isForeignKey = true;
                                                        relation rel = new relation();
                                                        rel.name = tab.Name + "_" + tab2.Name;
                                                        if (!pr.relations.Contains(rel.name))
                                                        {
                                                            rel.parentTable = tab2.Name;
                                                            rel.parentField = campo2.Name;

                                                            rel.childTable = tab.Name;
                                                            rel.childField = campo.Name;

                                                            // found description of fields...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.ToLower().Equals(rel.childTable.ToLower()))
                                                                    rel.childDescription = item.fieldDescription;

                                                                if (item.Name.ToLower().Equals(rel.parentTable.ToLower()))
                                                                    rel.parentDescription = item.fieldDescription;
                                                            }

                                                            pr.relations.Add(rel);

                                                            // now if the relation has to do with the tables...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.Equals(tab2.Name))
                                                                {
                                                                    // see if the relation exists..
                                                                    bool seguir = true;
                                                                    foreach (relation rel2 in tab2.relations)
                                                                    {
                                                                        if (rel2.name.ToLower().Equals(rel.name.ToLower()))
                                                                            seguir = false;
                                                                    }
                                                                    if (seguir)
                                                                        item.relations.Add(rel);
                                                                }

                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            right = true;
                            pr.host = pro.host;
                            pr.database = pro.database;
                            pr.user = pro.user;
                            pr.password = pro.password;
                            pr.dbDataType = pro.dbDataType;

                        }
                        else
                        {
                            AsyncWriteLine(errorMessage);
                        }
                        break;

                        case   project.databaseType.access2007:


                        connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pro.database + ";Persist Security Info=False;";
                                            //Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pro.database + ";Jet OLEDB:Database Password=MyDbPassword;
                        dbAccess dba2007 = new dbAccess();
                        errorMessage = dba2007.test(connectionString);
                        if (errorMessage.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            //pr = new project();
                            //pr.name = pro.name;

                            // lets get the tables...
                            List<table> lista = new List<table>();
                            lista = dba2007.getTables(connectionString, pro.database);
                            //lista.Sort();
                            foreach (table item in lista)
                            {
                                AsyncWriteLine("Found table... " + item.Name + "\n");

                                // now lets get the fields for each table...
                                List<field> listaField = new List<field>();
                                listaField = dba2007.getFields(connectionString, item.Name);
                                if (listaField != null)
                                {
                                    foreach (field fi in listaField)
                                    {
                                        item.fields.Add(fi);
                                        AsyncWriteLine("Found field... " + fi.Name + "\n");

                                    }

                                }

                                // lets get primary keys and foreign keys for the table...
                                dba2007.getKeys(connectionString, item);

                                // now we search a text field that is not key
                                if (listaField != null)
                                {
                                    // the descriptionField its the first string field of table...
                                    foreach (field campito in listaField)
                                    {
                                        if (campito.type.ToString().Equals("_string") || campito.type.ToString().Equals("_text") )
                                        {
                                            item.fieldDescription = campito.Name;
                                            break;
                                        }
                                    }

                                }
                                if (item.fieldDescription == null)
                                    item.fieldDescription = item.GetKey;

                                // lets sort the fields in the table...
                                // we order but put first key fields
                                if (general.orderFields)
                                {
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.name));
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.key));
                                }
                                pr.tables.Add(item);


                            }

                            pr.tables.Sort();
                            // now lets get the relations ...
                            List<relation> listarelation = new List<relation>();
                            listarelation = dba2007.getRelations(connectionString);
                            if (listarelation != null)
                            {
                                foreach (relation re in listarelation)
                                {
                                    //  item.fields.Add(re);
                                    pr.relations.Add(re);
                                    AsyncWriteLine("Found relation... " + re.name + "\n");

                                    // now if the relation has to do with the tables...
                                    foreach (table item in pr.tables)
                                    {
                                        // we put the relation in the parent table...
                                        if (item.Name.ToLower().Equals(re.parentTable.ToLower()))
                                        {
                                            // le añadimos la descripcion
                                            re.parentDescription = item.fieldDescription;

                                            foreach (table taby in pr.tables)
                                            {
                                                if (taby.Name.Equals(re.childTable))
                                                    re.childDescription = taby.fieldDescription;
                                            }
                                            item.relations.Add(re);
                                        }
                                    }


                                }

                            }


                            // also we can get relations about the field names
                            foreach (table tab in pr.tables)
                            {
                                foreach (field campo in tab.fields)
                                {
                                    if (campo.isKey)
                                    {
                                        foreach (table tab2 in pr.tables)
                                        {
                                            if (!tab.Name.ToLower().Equals(tab2.Name.ToLower()))
                                            {
                                                foreach (field campo2 in tab2.fields)
                                                {
                                                    if (campo.Name.ToLower().Equals(campo2.Name.ToLower()))
                                                    {
                                                        campo2.isForeignKey = true;
                                                        relation rel = new relation();
                                                        rel.name = tab.Name + "_" + tab2.Name;
                                                        if (!pr.relations.Contains(rel.name))
                                                        {
                                                            rel.parentTable = tab2.Name;
                                                            rel.parentField = campo2.Name;

                                                            rel.childTable = tab.Name;
                                                            rel.childField = campo.Name;

                                                            // found description of fields...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.ToLower().Equals(rel.childTable.ToLower()))
                                                                    rel.childDescription = item.fieldDescription;

                                                                if (item.Name.ToLower().Equals(rel.parentTable.ToLower()))
                                                                    rel.parentDescription = item.fieldDescription;
                                                            }

                                                            pr.relations.Add(rel);

                                                            // now if the relation has to do with the tables...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.Equals(tab2.Name))
                                                                {
                                                                    // see if the relation exists..
                                                                    bool seguir = true;
                                                                    foreach (relation rel2 in tab2.relations)
                                                                    {
                                                                        if (rel2.name.ToLower().Equals(rel.name.ToLower()))
                                                                            seguir = false;
                                                                    }
                                                                    if (seguir)
                                                                        item.relations.Add(rel);
                                                                }

                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            right = true;
                            pr.host = pro.host;
                            pr.database = pro.database;
                            pr.user = pro.user;
                            pr.password = pro.password;
                            pr.dbDataType = pro.dbDataType;

                        }
                        else
                        {
                            AsyncWriteLine(errorMessage);
                        }
                        break;
                        // end of access2007
                        case project.databaseType.excelOrCsv:

                        if (pr.database.Contains("xls"))
                            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pr.database + ";Extended Properties=\"text;HDR=Yes;FMT=Delimited\"";
                        else
                            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pr.database + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";


                        dbExcelOrCsv dbaODBC = new dbExcelOrCsv();
                        errorMessage = dbaODBC.test(connectionString);
                        if (errorMessage.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            //pr = new project();
                            //pr.name = pro.name;9

                            // lets get the tables...
                            List<table> lista = new List<table>();
                            lista = dbaODBC.getTables(connectionString, pr.database);
                            //lista.Sort();
                            foreach (table item in lista)
                            {
                                AsyncWriteLine("Found table... " + item.Name + "\n");

                                // now lets get the fields for each table...
                                List<field> listaField = new List<field>();
                                listaField = dbaODBC.getFields(connectionString, item.Name);


                                if (listaField != null)
                                {
                                    foreach (field fi in listaField)
                                    {
                                        item.fields.Add(fi);
                                        AsyncWriteLine("Found field... " + fi.Name + "\n");

                                    }

                                }

                                // lets get primary keys and foreign keys for the table...
                                dbaODBC.getKeys(connectionString, item);

                                // now we search a text field that is not key
                                if (listaField != null)
                                {
                                    // the descriptionField its the first string field of table...
                                    foreach (field campito in listaField)
                                    {
                                        if (campito.type.ToString().Equals("_string") || campito.type.ToString().Equals("_text"))
                                        {
                                            item.fieldDescription = campito.Name;
                                            break;
                                        }

                                    }

                                }
                                if (item.fieldDescription == null)
                                    item.fieldDescription = item.GetKey;


                                // lets sort the fields in the table...
                                // we order but put first key fields
                                if (general.orderFields)
                                {
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.name));
                                    item.fields.Sort(new compareFields(compareFields.CompareByOptions.key));
                                }
                                pr.tables.Add(item);


                            }

                            pr.tables.Sort();
                            // now lets get the relations ...
                            List<relation> listarelation = new List<relation>();
                            listarelation = dbaODBC.getRelations(connectionString);
                            if (listarelation != null)
                            {
                                foreach (relation re in listarelation)
                                {
                                    //  item.fields.Add(re);
                                    pr.relations.Add(re);
                                    AsyncWriteLine("Found relation... " + re.name + "\n");

                                    // now if the relation has to do with the tables...
                                    foreach (table item in pr.tables)
                                    {
                                        // we put the relation in the parent table...
                                        if (item.Name.ToLower().Equals(re.parentTable.ToLower()))
                                        {
                                            // le añadimos la descripcion
                                            re.parentDescription = item.fieldDescription;

                                            foreach (table taby in pr.tables)
                                            {
                                                if (taby.Name.Equals(re.childTable))
                                                    re.childDescription = taby.fieldDescription;
                                            }
                                            item.relations.Add(re);
                                        }
                                    }


                                }

                            }


                            // also we can get relations about the field names
                            foreach (table tab in pr.tables)
                            {
                                foreach (field campo in tab.fields)
                                {
                                    if (campo.isKey)
                                    {
                                        foreach (table tab2 in pr.tables)
                                        {
                                            if (!tab.Name.ToLower().Equals(tab2.Name.ToLower()))
                                            {
                                                foreach (field campo2 in tab2.fields)
                                                {
                                                    if (campo.Name.ToLower().Equals(campo2.Name.ToLower()))
                                                    {
                                                        campo2.isForeignKey = true;
                                                        relation rel = new relation();
                                                        rel.name = tab.Name + "_" + tab2.Name;
                                                        if (!pr.relations.Contains(rel.name))
                                                        {
                                                            rel.parentTable = tab2.Name;
                                                            rel.parentField = campo2.Name;

                                                            rel.childTable = tab.Name;
                                                            rel.childField = campo.Name;

                                                            // found description of fields...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.ToLower().Equals(rel.childTable.ToLower()))
                                                                    rel.childDescription = item.fieldDescription;

                                                                if (item.Name.ToLower().Equals(rel.parentTable.ToLower()))
                                                                    rel.parentDescription = item.fieldDescription;
                                                            }

                                                            pr.relations.Add(rel);

                                                            // now if the relation has to do with the tables...
                                                            foreach (table item in pr.tables)
                                                            {
                                                                if (item.Name.Equals(tab2.Name))
                                                                {
                                                                    // see if the relation exists..
                                                                    bool seguir = true;
                                                                    foreach (relation rel2 in tab2.relations)
                                                                    {
                                                                        if (rel2.name.ToLower().Equals(rel.name.ToLower()))
                                                                            seguir = false;
                                                                    }
                                                                    if (seguir)
                                                                        item.relations.Add(rel);
                                                                }

                                                            }
                                                        }

                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            right = true;
                            pr.host = pr.host;
                            pr.database = pr.database;
                            pr.user = pr.user;
                            pr.password = pr.password;
                            pr.dbDataType = pr.dbDataType;

                        }
                        else
                        {
                            AsyncWriteLine(errorMessage);
                        }
                        break;

                    // end of excelOrCsv

                }

                Cursor.Current = Cursors.Default;
               
                switch (right)
                {
                    case true:
                            AsyncWriteLine("All right. Now you can save the project...");
                            AsyncEnableButton(true);
                            SystemSounds.Exclamation.Play();
                            break;

                    case false:
                       
                            AsyncWriteLine("Error, review the configuration.");
                            AsyncEnableButton(false);
                            //util.playSimpleSound(Path.Combine(util.sound_dir, "zasentodalaboca.wav"));
                            SystemSounds.Asterisk.Play();
                            break;

                }

                // we have finished with new project
                Cursor.Current = Cursors.Default;

            }
            catch (Exception ex)
            {

                AsyncWrite(ex.Message);
            }

        }

        public void AsyncEnableButton(bool enabled)
        {
            try
            {
                butSaveChanges.BeginInvoke(new MethodInvoker(delegate
                {
                    butSaveChanges.Enabled = enabled;

                }));

            }
            catch (Exception exx)
            {
                //  AsyncWriteLine("Error: " + exx.Message.ToString() + "\n");
            }

        }

        public void AsyncWriteLine(String Text)
        {
            try
            {
                rt1.BeginInvoke(new MethodInvoker(delegate
                {

                    rt1.AppendText(Text + "\n");

                }));

            }
            catch (Exception exx)
            {
                //  AsyncWriteLine("Error: " + exx.Message.ToString() + "\n");
            }

        }

        public void AsyncWrite(String Text)
        {
            try
            {
                rt1.BeginInvoke(new MethodInvoker(delegate
                {

                    rt1.Text = Text + "\n";

                }));

            }
            catch (Exception exx)
            {
                //  AsyncWriteLine("Error: " + exx.Message.ToString() + "\n");
            }

        }

        private void rt1_TextChanged(object sender, EventArgs e)
        {
            SuspendLayout();
            Point pt = rt1.GetPositionFromCharIndex(rt1.SelectionStart);
            if (pt.Y > rt1.Height)
            {
                rt1.ScrollToCaret();
            }
            ResumeLayout(true);
        }

        private void newProject_Load(object sender, EventArgs e)
        {
            pr = new project();
        }

        private void cmbDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 
            //project.databaseType typeOfDatabase = new project.databaseType();
            //typeOfDatabase = (project.databaseType)cmbDataType.SelectedItem;
            switch ((project.databaseType)cmbDataType.SelectedItem)
            {
                case project.databaseType.dbf:
                    butAddDirectory.Visible = true;
                    txtHost.Text = "";
                    txtHost.Enabled = false;
                    break;
                case project.databaseType.access2003:
                    butAddDirectory.Visible = true;
                    txtHost.Text = "";
                    txtHost.Enabled = false;
                    break;
                case project.databaseType.access2007:
                    butAddDirectory.Visible = true;
                    txtHost.Text = "";
                    txtHost.Enabled = false;
                    break;
                case project.databaseType.mySql:
                    butAddDirectory.Visible = false;
                    txtHost.Enabled = true;
                    break;
                case project.databaseType.SqlServer:
                    butAddDirectory.Visible = false;
                    txtHost.Enabled = true;
                    break;
                case project.databaseType.excelOrCsv:
                    butAddDirectory.Visible = true;
                    txtHost.Text = "";
                    txtHost.Enabled = false;
                    break;
                case project.databaseType.dsn:
                    butAddDirectory.Visible = false;
                    txtHost.Text = "";
                    txtHost.Enabled = false;
                    break;
              
            }
        } // cmbDataType_SelectedIndexChanged

        private void butAddDirectory_Click(object sender, EventArgs e)
        {
            switch ((project.databaseType)cmbDataType.SelectedItem)
            {
                case project.databaseType.dbf:
                    // Display the openFile dialog.
                    DialogResult result =   folderBrowserDialog1.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        txtDatabase.Text = folderBrowserDialog1.SelectedPath;          
                    }
                    // Cancel button was pressed.
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                    break;
                case project.databaseType.access2003:
                    // Display the openFile dialog.
                    DialogResult result2 = openFileDialog1.ShowDialog();  
                    if (result2 == DialogResult.OK)
                    {
                        txtDatabase.Text = openFileDialog1.FileName;      
                    }
                    // Cancel button was pressed.
                    else if (result2 == DialogResult.Cancel)
                    {
                        return;
                    }
                    break;
                case project.databaseType.access2007:
                    // Display the openFile dialog.
                    DialogResult result3 = openFileDialog1.ShowDialog();
                    if (result3 == DialogResult.OK)
                    {
                        txtDatabase.Text = openFileDialog1.FileName;
                    }
                    // Cancel button was pressed.
                    else if (result3 == DialogResult.Cancel)
                    {
                        return;
                    }
                    break;

            }
            

        
        }

        private void butTest_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            // search metadata...
            Thread t = new Thread(new ParameterizedThreadStart(search));
            // we need to pass data through object
            //project pro = new project();
            pr.name = txtName.Text;
            pr.nameSpace = txtName.Text;
            pr.host = txtHost.Text;
            pr.database = txtDatabase.Text;
            pr.user = txtUser.Text;
            pr.password = txtPassword.Text;
            pr.dbDataType = (project.databaseType)cmbDataType.SelectedItem;

            t.Start(pr);

            // use this for debugging
            //search(pro);
        }

        private void butCancel2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void butSaveChanges_Click(object sender, EventArgs e)
        {
            if (pr != null)
            {
                // lets select some data from 
                if (pr.tables.Count > 0)
                {
                    pr.tableSelected = pr.tables[0].ToString();
                    pr.actualTable = (table)pr.tables[0];
                }

                pr.saveProject(Path.Combine(util.projects_dir, pr.name + ".xml"));
                AsyncWriteLine("Project saved... \n");

                this.DialogResult = DialogResult.Yes;
            }
        }

         

      


    }
}
