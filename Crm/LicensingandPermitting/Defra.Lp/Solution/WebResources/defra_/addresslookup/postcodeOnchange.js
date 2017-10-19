function postcodeOnChange() {
   var addressWebResource = Xrm.Page.getControl("WebResource_address");
   var src = addressWebResource.getSrc();
   addressWebResource .setSrc(null)
   addressWebResource .setSrc(src);
  //  Xrm.Page.getControl("WebResource_address").setSrc(Xrm.Page.getControl("WebResource_address").getSrc());
    Xrm.Page.getControl("WebResource_address").setVisible(true);
}