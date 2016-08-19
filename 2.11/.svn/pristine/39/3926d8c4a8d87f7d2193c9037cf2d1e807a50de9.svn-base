<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DummyLogin.aspx.cs" Inherits="Exceedra.Web.DummyLogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title></title>
 
    <script src="/Scripts/jquery-2.1.4.js"></script>
     <script> 
         $(document).ready(function () { 
             $("#Save").click(function () { 
                 var person = new Object();
                 person.username = $('#name').val();
                 person.password = $('#surname').val();
                 person.token = $('#token').val();
                  
                 $.ajax({
                     url: 'https://localhost:44300/api/SysConfig',
                     type: 'POST',
                     dataType: 'json',
                     data: person,
                     success: function (data, textStatus, xhr) {
                         console.log(data);
                         
                         $("#demo-container").html("<b>DB: " + data.connection + " - used everytime a gateway call is fired for this user</b><br />");
                         $("#demo-container").append("<b>Token: " + data.token + "</b> - gets passed back to app to be used in every call to gateway in the future (until user logs in elsewhere) <br />");
                         $("#demo-container").append("<b>Message: " + data.message + "</b><br />");
                         $("#token").val(data.token);
                     },
                     error: function (xhr, textStatus, errorThrown) {
                         console.log('Error in Operation');
                     }
                 });
             });
         });
    </script>
</head>
<body>
    <form id="form1">
        Name :- <input type="text" name="name" id="name" /><br/>
        Surname:- <input type="text" name="surname" id="surname" /><br/>
        Token:- <input type="text" name="token" id="token" /><br/>
        <input type="button" id="Save" value="Save Data" />
       <div id="demo-container">

         
        </div>


    </form>
</body>
</html>


