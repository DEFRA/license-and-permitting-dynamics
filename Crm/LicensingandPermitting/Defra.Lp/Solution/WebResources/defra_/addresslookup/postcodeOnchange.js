function postcodeOnChange() {
    Xrm.Page.getControl("WebResource_address").setSrc(Xrm.Page.getControl("WebResource_address").getSrc());
    Xrm.Page.getControl("WebResource_address").setVisible(true);
}