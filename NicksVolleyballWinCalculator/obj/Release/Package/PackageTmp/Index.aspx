<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="NicksVolleyballWinCalculator.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Social Sports Stats - How to Take a League Too Serious</title>
    <link runat="server" rel="shortcut icon" href="myIcon.ico" type="image/x-icon"/>
    <link runat="server" rel="icon" href="myIcon.ico" type="image/ico"/>
    <style type="text/css">
        .container{
            display: flex;
            flex-direction: column;
            align-items: center;
            width:100%;
        }
        body{
            background-image: url(http://guyanachronicle.com/wp-content/uploads/2016/01/3ds_grass_texture241_173.jpg);
            background-size: cover;
            background-attachment:fixed;
        }
        .urlTextBox{
            font-size:40px;
            width:380px;

        }
        .table{
            background:rgba(255,255,255,.7);
        }

        .percentlbl{
            background:rgba(255,255,255,.7);
            display:inline-block;
            max-width:1100px;
            margin-bottom:10px;
        }

        .leagueTitle{
            display:inline-block;
            max-width:1100px;
        }
        .TeamTable{
            /*float:left;*/
            padding-right:10px;
            text-align:left;
            flex: 0 0 auto;
        }

        .StandingsTable{
            background:rgba(255,255,255,.7);
            text-align:left;
            /*margin:0 auto;*/ 
            display: inline-block; 
            flex: 0 0 auto;
            /*float:right;*/ 
        }
        .matchupTeam1{
            background:rgba(255,255,255,.7);
            /*float:left;*/
            text-align:left;
            border:solid;
            border-width:1px;
            flex: 0 0 auto;
        }
        .matchupTeam2{
            background:rgba(255,255,255,.7);
            text-align:left;
            /*margin:0 auto;*/ 
            display: inline-block;
            /*float:right;*/
            border:solid;
            border-width:1px;
            flex: 0 0 auto;
        }
        .vsBtn
        {
            margin-left:10px;
            margin-right:10px;

        }
        .topRow{
            display: flex;
            justify-content: center;
            flex-wrap: wrap;
        }
        .secondRow{
            display: flex;
            justify-content: center;
            align-items:center;
        }
        @media only screen and (max-width: 1024px) {
            body{
                background:rgb(14, 47, 102);
            }
            
            .leagueTitle{
                display:inline-block;
                max-width:1100px;
                color:white;
            }
            .urlTextBox{
                font-size:60px !important;
                width:90%;
            }
            .urlBtn{
                font-size:40px !important;
                height:50px !important;
            }
            .TeamTable{
                text-align:left;
                margin:0 auto; 
                display: inline-block;
                float:none;
            }
            .StandingsTable{
                background:rgba(255,255,255,.7);
                text-align:left;
                margin:0 auto; 
                display: inline-block;
                float:none;
                margin-top: 10px;
            }
        }
     </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="container">
        <div style="text-align:center;">
            <asp:TextBox CssClass="urlTextBox" id="urlTextBox" placeholder="https://app.mysocialsports.com/leagues/999" Font-Size="14" runat="server" />
            <br />
            <asp:Button CssClass="urlBtn" ID="urlBtn" Height="28" runat="server" onclick="urlBtn_Click" Text="Change League" />
            <br />
            <asp:Label ID ="logging" Text="" Font-Size="Larger"  runat="server"/>
            <br />
            <asp:Label ID ="League" CssClass="leagueTitle" Text="" Font-Size="XX-Large"  runat="server"/>
            <br />
            <br />
            <%--Team Table--%>
            <div class="topRow">
                <div class="TeamTable">
                    <asp:Table ID="TeamTable" GridLines="Horizontal" CellPadding="5" runat="server">
                        <asp:TableHeaderRow runat="server" Font-Size="Larger">
                            <asp:TableHeaderCell CssClass="table" >Name</asp:TableHeaderCell>
                            <asp:TableHeaderCell CssClass="table" >Rating</asp:TableHeaderCell>
                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>
            <%--Standings Table--%>
                <div class="StandingsTable">
                    <asp:Table ID="StandingsTable" GridLines="Horizontal" HorizontalAlign="Center" CellPadding="5" runat="server">
                    </asp:Table>
                </div>
            </div>
            <br />
        </div>
            <%--Match Table--%>
            <asp:Table ID="MatchTable" GridLines="Horizontal" CssClass="table" HorizontalAlign="Center" Visible="true" CellPadding="5" runat="server">
            </asp:Table>
        </div>
    </form>
</body>
</html>
