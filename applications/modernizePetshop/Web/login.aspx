<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<asp:Content ID="cntPage" ContentPlaceHolderID="cphPage" Runat="Server" EnableViewState="false" >
    
    <script type="text/javascript" src="https://www.gstatic.com/firebasejs/live/4.1/firebase.js"></script>
    <script type="text/javascript" src="config.js"></script>
    <script type="text/javascript" src="common.js"></script>
    <script type="text/javascript"  src="https://cdn.firebase.com/libs/firebaseui/2.2.1/firebaseui.js"></script>
    <link type="text/css" rel="stylesheet" href="https://cdn.firebase.com/libs/firebaseui/2.2.1/firebaseui.css" />
    
    <div id="placeHolderDiv" style="height:280px;width:626px;background-color:#F5F5F5">
    <div id="container" style="background-color:#F5F5F5">
      <div style="height:60px;line-height:60px; margin-bottom: 1cm;text-align:center;background-color:#0288d1;color:white">
          <div>
              <img alt="Google Firebase Authentication" style="text-align:left; border:0" src="Comm_Images/firebase_28dp.png"/>
              <img  alt="Firebase" src="http://www.gstatic.com/mobilesdk/160323_mobilesdk/images/firebase_logotype_white_18dp.svg" />
          </div>

      </div>
      <div id="loading">Loading...</div>
      <div id="loaded" class="hidden">
        <div id="main">
          <div id="user-signed-in" class="hidden">
            <div id="user-info">

              <div id="name"></div>
              <div id="email"></div>
              <div id="phone"></div>
              <div class="clearfix"></div>
            </div>
            <p>
              <button id="sign-out">Sign Out</button>
              <button id="delete-account">Delete account</button>
            </p>
          </div>
          <div id="user-signed-out" class="hidden">
            
            <div id="firebaseui-spa">
              <!--<h5>Single Page Application mode:</h5>-->
              <div id="firebaseui-container"></div>
            </div>
          </div>
        </div>
       
      </div>

        <div id="sign-in-status"></div>
        <div id="sign-in"></div>
        <div id="account-details"></div>
    </div>
    </div>
    <script type="text/javascript" src="app.js"></script>
</asp:Content>
