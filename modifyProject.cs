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
    public partial class modifyProject : Form
    {

        public project pr;

        public modifyProject()
        {
            InitializeComponent();

            // fill the combobox with enum...

            cmbDataType.DataSource = Enum.GetValues(typeof(project.databaseType));


            //if (pr != null)
            //{
            //    txtName.Text = pr.name;
            //    txtHost.Text = pr.host;
            //    txtDatabase.Text = pr.database;
            //    txtUser.Text = pr.user;
            //    txtPassword.Text = pr.password;
            //    cmbDataType.SelectedItem = pr.dbDataType.ToString();


            //}

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            // search metadata...
            Thread t = new Thread(new ParameterizedThreadStart(search));
            //// we need to pass data through object
            //project pro = new project();
            //pro.name = txtName.Text;
            //pro.host = txtHost.Text;
            //pro.database = txtDatabase.Text;
            //pro.user = txtUser.Text;
            //pro.password = txtPassword.Text;
            //pro.dbDataType = (project.databaseType)cmbDataType.SelectedItem;

            t.Start(pr);

        }

        private void newProject_Load(object sender, EventArgs e)
        {
            if (pr != null)
            {
                txtName.Text = pr.name;
                txtHost.Text = pr.host;
                txtDatabase.Text = pr.database;
                txtUser.Text = pr.user;
                txtPassword.Text = pr.password;

                int index = cmbDataType.FindStringExact(pr.dbDataType.ToString());
                cmbDataType.SelectedIndex = index;

                //cmbDataType.SelectedItem = pr.dbDataType.ToString();
            }
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            
        }


        private void search(object file)
        {

            bool right = false;

            AsyncEnableButton(false);
            try
            {
                //project pro = new project();
                //pro = (project)file;

                String errorMessage = null;

                Cursor.Current = Cursors.WaitCursor;

                switch (pr.dbDataType)
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
                                        {
                                            re.parentDescription = item.fieldDescription;
                                           
                                        }
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
                                                        campo2.isForeignKey = true;

                                                        // check if relation exists..
                                                        if (!pr.existsRelation(tab.Name, tab2.Name))
                                                        {

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


                            //pr.host = pro.host;
                            //pr.database = pro.database;
                            //pr.user = pro.user;
                            //pr.password = pro.password;

                            //pr.saveProject(Path.Combine(util.projects_dir, pro.name + ".xml"));

                            // lets save it for next load of application...
                            // pr.saveProject(Path.Combine(util.projects_dir, "conf.xml"));
                            //  AsyncWriteLine("Project saved... \n");


                            right = true;
                        }
                        else
                        {
                            AsyncWrite(errorMessage);
                        }
                        break;

                    case project.databaseType.SqlServer:
                        //String connectionStringSqlServer = null;

                        String messageSqlServer = null;


                        connectionString = "Data Source=" + pr.host + ";Network Library=DBMSSOCN;Initial Catalog=" + pr.database + ";User ID=" + pr.user + ";Password=" + pr.password + ";";
                        // connectionStringOleDb = "Provider=SQLNCLI;Server=" + txtHost.Text + ";Database=" + txtDatabase.Text + ";Uid=" + txtUser.Text + ";Pwd=" + txtPassword.Text + ";";



                        dbSql2005 dbSqlServer = new dbSql2005();
                        messageSqlServer = dbSqlServer.test(connectionString);
                        if (messageSqlServer.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            // pr = new project();
                            pr.name = pr.name;

                            // lets get the tables...
                            pr.tables.Clear();
                            List<table> lista = new List<table>();
                            lista = dbSqlServer.getTables(connectionString, pr.database);
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
                            pr.relations.Clear();
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
                                        if (item.Name.ToLower().Equals(re.childTable.ToLower()))
                                            re.childDescription = item.fieldDescription;

                                        if (item.Name.ToLower().Equals(re.parentTable.ToLower()))
                                        {
                                            // le añadimos la descripcion
                                            re.parentDescription = item.fieldDescription;
                                            // we put the relation in the parent table...
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

                                                        // to know if exists
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

                            //pr.host = pro.host;
                            //pr.database = pro.database;
                            //pr.user = pro.user;
                            //pr.password = pro.password;

                            //pr.saveProject(Path.Combine(util.projects_dir, pro.name + ".xml"));

                            // lets save it for next load of application...
                            // pr.saveProject(Path.Combine(util.projects_dir, "conf.xml"));
                            //AsyncWriteLine("Project saved... \n");

                            right = true;

                        }
                        else
                        {
                            AsyncWrite(messageSqlServer);
                        }

                        break;

                    case project.databaseType.dbf:


                        connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pr.database + ";Extended Properties=dBASE IV;User ID=" + pr.user + ";Password=" + pr.password + ";";

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
                            lista = dbf.getTables(connectionString, pr.database);
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

                    case project.databaseType.access2003:


                        connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pr.database + ";User ID=" + pr.user + ";Password=" + pr.password + ";";

                        dbAccess dba2003 = new dbAccess();
                        errorMessage = dba2003.test(connectionString);
                        if (errorMessage.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            //pr = new project();
                            //pr.name = pro.name;9

                            // lets get the tables...
                            List<table> lista = new List<table>();
                            lista = dba2003.getTables(connectionString, pr.database);
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

                    // end of access2003


                    case project.databaseType.access2007:


                        connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pr.database + ";Persist Security Info=False;";

                        dbAccess dba2007 = new dbAccess();
                        errorMessage = dba2007.test(connectionString);
                        if (errorMessage.Equals(""))
                        {
                            AsyncWrite("");
                            AsyncWriteLine("Success connection \n");
                            //pr = new project();
                            //pr.name = pro.name;9

                            // lets get the tables...
                            List<table> lista = new List<table>();
                            lista = dba2007.getTables(connectionString, pr.database);
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
                                        if (campito.type.ToString().Equals("_string") || campito.type.ToString().Equals("_text"))
                                        {
                                            item.fieldDescription = campito.Name;
                                            campito.isDescriptiveField = true;
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

                    // end of access2007

                    case project.databaseType.excelOrCsv:

                        if(pr.database.Contains("xls"))
                            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pr.database + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                        else
                            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pr.database + ";Extended Properties=\"text;HDR=Yes;FMT=Delimited\"";


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
                        AsyncCleanMessage();
                        AsyncWriteMessage("All right. Now you can save the project...", Color.GreenYellow);
                        // timer1.Enabled = true;
                        break;

                    case false:
                        AsyncWriteLine("Error, review the configuration.");
                        AsyncEnableButton(false);
                        SystemSounds.Asterisk.Play();
                        AsyncCleanMessage();
                        AsyncWriteMessage("Error, review the configuration....", Color.Red);
                        break;

                }

            }
            catch (Exception ex)
            {
                SystemSounds.Asterisk.Play();
                AsyncWrite(ex.Message);

            }
        }


        public void AsyncEnableButton(bool enabled)
        {
            try
            {
                butSave.BeginInvoke(new MethodInvoker(delegate
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


        public void AsyncWriteMessage(String Text, Color color)
        {
            try
            {

                rtMessage.BeginInvoke(new MethodInvoker(delegate
                {
                    rtMessage.AppendText(Text + "\n");
                    rtMessage.BackColor = color;

                }));
                timer1.Enabled = true;
                timer1.Start();

            }
            catch (Exception exx)
            {
                //  AsyncWriteLine("Error: " + exx.Message.ToString() + "\n");
            }

        }


        public void AsyncCleanMessage()
        {
            try
            {

                rtMessage.BeginInvoke(new MethodInvoker(delegate
                {
                    rtMessage.Clear();
                    rtMessage.BackColor = Color.Wheat;

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

        private void butSaveChanges_Click(object sender, EventArgs e)
        {
            
        }

        private void butAddDirectory_Click(object sender, EventArgs e)
        {
            switch ((project.databaseType)cmbDataType.SelectedItem)
            {
                case project.databaseType.dbf:
                    // Display the openFile dialog.
                    DialogResult result = folderBrowserDialog1.ShowDialog();
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

                case project.databaseType.excelOrCsv:
                    // Display the openFile dialog.
                    DialogResult result4 = openFileDialog1.ShowDialog();
                    if (result4 == DialogResult.OK)
                    {
                        txtDatabase.Text = openFileDialog1.FileName;
                    }
                    // Cancel button was pressed.
                    else if (result4 == DialogResult.Cancel)
                    {
                        return;
                    }
                    break;


            }
        } // butAddDirectory_Click

        private void cmbDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            // limpiamos el message
            rtMessage.Text = "";
            rtMessage.BackColor = Color.WhiteSmoke;
            timer1.Enabled = false;

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void butSave_Click_1(object sender, EventArgs e)
        {
            //http://207.46.16.248/en-us/library/MySql.Data.MySqlClient.sqlclientmetadatacollectionnames_fields%28VS.100%29.aspx


            //return DialogResult.OK;


            // search metadata...
            Thread t = new Thread(new ParameterizedThreadStart(search));
            // we need to pass data through object
            //project pro = new project();
            if (pr == null)
                pr = new project();

            pr.name = txtName.Text;
            pr.host = txtHost.Text;
            pr.database = txtDatabase.Text;
            pr.user = txtUser.Text;
            pr.password = txtPassword.Text;
            pr.dbDataType = (project.databaseType)cmbDataType.SelectedItem;

            //project.databaseType tipo = new project.databaseType();
            //tipo = (project.databaseType)cmbDataType.SelectedItem;


            //pro.dbDataType = cmbDataType.SelectedItem;


            // clear data 
            pr.tables.Clear();
            pr.relations.Clear();

            t.Start(pr);

            // this.DialogResult = DialogResult.Yes;

            //MessageBox.Show("Hand message", "Hand title", MessageBoxButtons.OK, MessageBoxIcon.Hand);

        }

        private void butSaveChanges_Click_1(object sender, EventArgs e)
        {
            //pr.host = pro.host;
            //pr.database = pro.database;
            //pr.user = pro.user;
            //pr.password = pro.password;

            pr.saveProject(Path.Combine(util.projects_dir, pr.name + ".xml"));
            AsyncWriteLine("Project saved... \n");

            this.DialogResult = DialogResult.Yes;
        }

        private void butCancel2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       


    }
}
