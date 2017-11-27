//var blankWebResource = Xrm.Page.context.getClientUrl() + "/WebResources/defra_defra_/addresslookup/addressuiblank.html";
var lkpWebResource = Xrm.Page.context.getClientUrl() + "/WebResources/defra_/addresslookup/addressui.html"

function PostcodeChange(){
	var addressWebResource = Xrm.Page.getControl("WebResource_address");
	var src = addressWebResource.getSrc();
	
	addressWebResource.setSrc(null)
	addressWebResource.setSrc(lkpWebResource);
    addressWebResource.setVisible(true);
}