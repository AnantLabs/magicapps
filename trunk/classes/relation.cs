//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión del motor en tiempo de ejecución:2.0.50727.3603
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using System;


 


	public class relation
	{

        public enum typeOfRelation
        {
            hasOne=0,
            hasMany=1,
            belongsTo=2,
            hasAndBelongToMany=3
        }
       

		#region["Variables"]		 
		 
			     private string _name;
			     private string _parentTable;
			     private string _childTable;
			     private string _parentField;
			     private string _childField;
			     private string _parentDescription;
			     private string _childDescription;
			     private int _cardinality;
                 private typeOfRelation _relationType;


	
	#endregion
	
	
	#region["Propiedades"]
	
	
			     
			      public string name
		                {
			                get {return _name;}
			                set {_name = value;}			                
		                }
			      public string parentTable
		                {
			                get {return _parentTable;}
			                set {_parentTable = value;}			                
		                }
			      public string childTable
		                {
			                get {return _childTable;}
			                set {_childTable = value;}			                
		                }
			      public string parentField
		                {
			                get {return _parentField;}
			                set {_parentField = value;}			                
		                }
			      public string childField
		                {
			                get {return _childField;}
			                set {_childField = value;}			                
		                }
			      public string parentDescription
		                {
			                get {return _parentDescription;}
			                set {_parentDescription = value;}			                
		                }
			      public string childDescription
		                {
			                get {return _childDescription;}
			                set {_childDescription = value;}			                
		                }
			      public int cardinality
		                {
			                get {return _cardinality;}
			                set {_cardinality = value;}
		                }

                  public typeOfRelation relationType
                  {
                      get { return _relationType; }
                      set { _relationType = value; }
                  }
	  				
	  				 
	  				  
	  		  			
				
    
    #endregion
		
		
		public relation ()
		{
			
			
		}
	}
 
