// ================= CARD DATA =================
async function loadDashboardCardData() {

    const url = `/LinePerformanceDashboard/GetDashboardCardData?t=${Date.now()}`;

    const data = await apiGet(url);

    if (!Array.isArray(data) || data.length === 0) return;

    const d = data[0];

    const map = {
        date: d.proD_DATE,
        lineNo: d.line_NAME,
        itemName: d.iteM_NAME,
        buyerName: d.buyeR_NAME,
        styleRef: d.stylE_REF_NO,
        avgSmv: d.avG_SMV,
        manPower: d.maN_POWER
    };

    Object.entries(map).forEach(([id, value]) => {
        const element = $(id);

        if (element) {
            element.innerText = value ?? 0;
        }
    });
}
document.addEventListener("DOMContentLoaded", function () {
    loadDashboardCardData();
});