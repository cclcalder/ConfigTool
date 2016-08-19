<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="file.aspx.cs" Inherits="Website.deployApp.log.file" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <table width="600">
        <tr>
            <td colspan="2"><strong>Server path:</strong> <% =LocalPath %>
            </td>
        </tr>
        <tr>
            <td colspan="2"><strong>Optional Folder:</strong>
            
                <asp:TextBox runat="server" ID="TextBox1" Width="251px"></asp:TextBox></td>
        </tr>
      
        <tr>
            <td></td>
            <td></td>
        </tr>
        <tr>
        <td width="300">

            <table width="290">
                <tr><td colspan="2">
                    <h3>Save to txt file</h3>
                </td></tr>
                <tr>
            <td width="100"><strong>Text file name</strong></td>
            <td style="text-align: right">
                <asp:TextBox runat="server" ID="TextBox2"></asp:TextBox>
            </td>
        </tr>
                <tr>
                    <td colspan="2">
                        <strong>Contents</strong><br />
                        <asp:TextBox runat="server" TextMode="MultiLine" ID="TextBox3" Height="300" Width="100%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td align="right">
                        <asp:Button runat="server" Text="Save" OnClick="OnClick" />
                    </td>
                </tr>
            </table>

        </td>
        <td width="300" style="vertical-align: top; border-left: 1pt solid red; padding-left:10px" >

           <H3> Upload file </H3>

            <asp:FileUpload runat="server" ID="UploadTest" />
            <asp:Button runat="server" Text="Upload" OnClick="OnClick2" />
            <br />
            <asp:Label runat="server" ID="UploadDetails"  style="color: red"></asp:Label>

        </td>
        </tr>
    </table>


</asp:Content>
