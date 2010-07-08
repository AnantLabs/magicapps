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
using System.Collections;

// para properties del propertyGrid
using System.ComponentModel;

[DefaultPropertyAttribute("name")]
public class field : IComparable
{

    public enum fieldType
    {
        _text,
        _string,
        _integer,
        _hidden,
        _tinyInt,
        _long,
        _decimal,
        _double,
        _date,
        _datetime,
        _boolean,
        _blob,
        _varchar,
        _guid,
        _money,
        _image,
        _document,
        _audio,
        _video,
        _uniqueidentifier

    }

    #region["Variables"]


    private string name;
    private fieldType _type;
    private fieldType _targetType;
    private string _targetName;
    private string _comment;
    private bool _selected;

    private bool _isKey;

    private bool _isForeignKey;

    private bool _allowNulls;

    private bool _isAutoIncrement;
    private bool _isDescriptionInCombo;
    private string _defaultValue;
    private int _size;
 
    private string _getQuoted;

    private bool _changed;

    private bool _autoNumber;

    private bool _buttonInDataGrid;
    private int _decimals;

    private bool _imageInDataGrid;
    private bool _visibleInGrid;
    private bool _invisible; // if its invisible in pages...


    private bool _isSearchable; // if its searchable in pages...

    private bool _nameChanged;
    private bool _createdNew;
    private bool _deleted;

    // random values
    private int _randomMinValue=0;
    private int _randomMaxValue = 100;



    // This attribute enables the ArrayList to be serialized:
    [System.Xml.Serialization.XmlArray("ValidationRules")]
    // Explicitly tell the serializer to expect the Item class
    // so it can be properly written to XML from the collection:
    [System.Xml.Serialization.XmlArrayItem("validationRule", typeof(validationRule))]
    public ArrayList validationRules = new ArrayList();

    #endregion


    #region["Propiedades"]


   
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public fieldType type
    {
        get { return _type; }
        set { _type = value; }
    }
    public fieldType targetType
    {
        get { return _targetType; }
        set { _targetType = value; }
    }


    // Name property with category attribute and
    // description attribute added
    [CategoryAttribute("Extra"), DescriptionAttribute("Name that appears at form")]
    public string targetName
    {
        get { return _targetName; }
        set { _targetName = value; }
    }
 

    public bool selected
    {
        get { return _selected; }
        set { _selected = value; }
    }


    public bool isKey
    {
        get { return _isKey; }
        set { _isKey = value; }
    }

    public bool isForeignKey
    {
        get { return _isForeignKey; }
        set { _isForeignKey = value; }
    }

    public bool allowNulls
    {
        get { return _allowNulls; }
        set { _allowNulls = value; }
    }

    public bool isAutoIncrement
    {
        get { return _isAutoIncrement; }
        set { _isAutoIncrement = value; }
    }
    
    public bool isDescriptionInCombo
    {
        get { return _isDescriptionInCombo; }
        set { _isDescriptionInCombo = value; }
    }

    public string defaultValue
    {
        get { return _defaultValue; }
        set { _defaultValue = value; }
    }
    public int size
    {
        get { return _size; }
        set { _size = value; }
    }
    [CategoryAttribute("Descripcion"), DescriptionAttribute("Name that appears at form")]

    public string comment
    {
        get { return _comment; }
        set { _comment = value; }
    }
    public string getQuoted
    {
        get { return _getQuoted; }
        set { _getQuoted = value; }
    }

    [CategoryAttribute("Fixed"),
    ReadOnlyAttribute(true)]
    public bool changed
    {
        get { return _changed; }
        set { _changed = value; }
    }

    public bool autoNumber
    {
        get { return _autoNumber; }
        set { _autoNumber = value; }
    }

  

    public int decimals
    {
        get { return _decimals; }
        set { _decimals = value; }
    }

     public bool buttonInDataGrid
    {
        get { return _buttonInDataGrid; }
        set { _buttonInDataGrid = value; }
    } 
    
    public bool imageInDataGrid
    {
        get { return _imageInDataGrid; }
        set { _imageInDataGrid = value; }
    }

    public bool invisible
    {
        get { return _invisible; }
        set { _invisible = value; }
    }

    public bool visibleInGrid
    {
        get { return _visibleInGrid; }
        set { _visibleInGrid = value; }
    }


    public bool isSearchable
    {
        get { return _isSearchable; }
        set { _isSearchable = value; }
    }

    // if we change the name of the field...
    public Boolean nameChanged
    {
        get { return _nameChanged; }
        set { _nameChanged = value; }
    }

    // if we create the field in the model editor
    public Boolean createdNew
    {
        get { return _createdNew; }
        set { _createdNew = value; }
    }


    // if we delete the field in the model editor
    public Boolean deleted
    {
        get { return _deleted; }
        set { _deleted = value; }
    }



    // le decimos al serializer que lo ignore o nos duplica los campos...
    [System.Xml.Serialization.XmlIgnore]
    public ArrayList GetArrayOfValidationRules
    {
        get { return validationRules; }
        set { validationRules = value; }
    }


    // variables used to test and to fill the database with random data
    public int randomMinValue
    { 
        get { return _randomMinValue; }
        set { _randomMinValue = value; }
    }
    public int randomMaxValue
    {
        get { return _randomMaxValue; }
        set { _randomMaxValue = value; }
    }



    #endregion


    public field()
    {

        validationRules = new ArrayList();

    }


    // para poder ordenar...
    public int CompareTo(object obj)
    {
        field Compare = (field)obj;
        return string.Compare(this.name, Compare.name);

    }


    public override string ToString()
    {
        return this.Name;
    }




}

