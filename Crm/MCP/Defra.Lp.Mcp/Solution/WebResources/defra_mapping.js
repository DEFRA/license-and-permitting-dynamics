/************************************************************
        * WE-2008 (MCP Mapping) Adding Button to Ribbon instead of using Web resources.
               *Developments for MCP Plant Location Map checking
               * DRAFT VERSION OF showing Google Map and EasiMap for MCP
               * Code below is not finished NOR tested.. 
               *by Kassim Hassan
               ************************************************************/

const arcgisString = "https://easimap.prodds.ntnl/Silverlight/?Viewer=permitting&showAdvancedTools=True&showDataFrame=False&extent=%20411793,%20332833,%20%20413793,%20333533&layerTheme=1&layers=79,0,1,0,72,0,0,121,0,0,73,0,5,0,1,2,3,4,80,0,0,120,0,1,0,114,0,3,0,1,2,22,0,0,78,0,1,1,52,0,3,3,4,5,86,1,0,40,0,0,111,0,2,23,24,75,0,5,1,2,3,4,5,45,0,11,2,3,5,6,8,7,10,11,13,14,0,116,0,20,2,20,21,24,27,30,36,37,39,40,42,43,45,46,48,49,51,52,54,55,118,0,0,117,0,0,74,0,1,0,50,0,3,29,30,31,66,1,0,92,1,0,76,0,3,2,3,4,82,0,6,5,13,22,30,38,41,83,0,0,64,0,2,0,1,65,0,2,1,3,85,0,0,115,1,0,84,0,0,90,1,0,91,1,0,69,0,0,89,0,0,88,0,0,87,0,0,&";

/* do conversion code goes here..... */
let convertGridRef = gridrefFromCRM => ({ convertGridRef: 1 * gridrefFromCRM });

function openGoogle() {
    try {
        var crmLongVal = Xrm.Page.getAttribute("defra_plantlongitude").getValue();
        var crmLatVal = Xrm.Page.getAttribute("defra_plantlatitude").getValue();
        var googleString = "https://www.google.co.uk/maps/place/@" + crmLatVal + "," + crmLongVal + ",293m/data=!3m1!1e3!4m5!3m4!1s0x0:0x0!8m2!3d51.4299!4d-0.0323";
        return googleString;

    }
    catch (err) {
        console.error('Error captured in openGoogle: ', err);
    }
}


function openEasiMap() {
    try {
        return arcgisString;

    }
    catch (err) {
        console.error('Error captured in openEasiMap: ', err);
    }
}

