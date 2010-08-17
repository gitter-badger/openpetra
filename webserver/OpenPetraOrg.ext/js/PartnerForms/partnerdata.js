Ext.onReady(function(){
    Ext.QuickTips.init();

    // use our own blank image to avoid a call home
    // Ext.BLANK_IMAGE_URL = 'resources/images/default/s.gif';

    Ext.form.Field.prototype.msgTarget = 'side';

    CreateCustomValidationTypePassword();

    var ItemsOnForm = {
    items:[
        {
            layout:'column',
            border:false,
            items: [{
            columnWidth:0.5,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'First name',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtFirstName',
            anchor: '95%'
            }
        ]
            }
        ,{
            columnWidth:0.5,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Last Name',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtLastName',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Street',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtStreet',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:0.5,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Postcode',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtPostcode',
            anchor: '95%'
            }
        ]
            }
        ,{
            columnWidth:0.5,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'City',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtCity',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Phone',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtPhone',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Email',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtEmail',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Password',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtPassword',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Password Confirm',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtPasswordConfirm',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            xtype: 'fieldset',
            columnWidth: 1.0,
            border:false,
            items: [{
            xtype: 'radio',
            boxLabel: 'Pupil',
            fieldLabel: 'Employment Status',
            allowBlank: true,
            name: 'rgrEmploymentStatus',
            anchor: '95%'
            }
        ,{
            xtype: 'radio',
            boxLabel: 'Student',
            fieldLabel: '',
            allowBlank: true,
            name: 'rgrEmploymentStatus',
            anchor: '95%'
            }
        ,{
            xtype: 'radio',
            boxLabel: 'Unemployed',
            fieldLabel: '',
            allowBlank: true,
            name: 'rgrEmploymentStatus',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Profession',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtProfession',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Kontoinhaber',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtKontoinhaber',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Kontonummer',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtKontonummer',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Iban',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtIban',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Kreditinstitut',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtKreditinstitut',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ,{
            layout:'column',
            border:false,
            items: [{
            columnWidth:1,
            layout: 'form',
            border:false,
            items: [{
            xtype: 'textfield',
            fieldLabel: 'Ort',
            allowBlank: true,
            Width: -1,
            emptyText:'TODO',
            name: 'txtOrt',
            anchor: '95%'
            }
        ]
            }
        ]
        }
        ]}

    var partnerdata = new Ext.FormPanel({
        frame: true,
        // monitorValid:true,
        title: 'partnerdata',
        bodyStyle: 'padding:5px',
        width: 650,
        labelWidth: 140,
        items: [{
            bodyStyle: {
            margin: '0px 0px 15px 0px'
        },
        items: ItemsOnForm
        }],
    buttons: [{text: 'Cancel'}]
    });

    partnerdata.render('partnerdata');
})