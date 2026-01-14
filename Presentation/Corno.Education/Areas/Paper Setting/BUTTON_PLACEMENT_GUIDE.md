<!-- 
TRANSFER BUTTON PLACEMENT GUIDE

Add this button to the card header of each Create view for consistent UX
-->

<!-- BUTTON HTML SNIPPET (Copy to each Create view header) -->
<button type="button" id="btnOpenTransfer" class="k-button k-button-md k-rounded-lg k-button-outline k-button-outline-info">
    <span><i class="fa fa-exchange"></i></span> Transfer from old Instance
</button>

<!-- 
COMPLETE HEADER EXAMPLE (use in Create.cshtml)
-->

<div class="k-card k-card-vertical">
    <div class="k-card-header k-card-tertiary" style="display: flex; justify-content: space-between; align-items: center;">
        <h3 style="margin: 0;">Entity Name (Appointment/Schedule/Remuneration)</h3>
        <button type="button" id="btnOpenTransfer" class="k-button k-button-md k-rounded-lg k-button-outline k-button-outline-info">
            <span><i class="fa fa-exchange"></i></span> Transfer from old Instance
        </button>
    </div>
    <div class="k-card-body">
        <!-- Your form content here -->
    </div>
</div>

<!-- 
AT THE END OF EACH Create.cshtml, RENDER THE TRANSFER PARTIAL
-->

@{
    Html.RenderPartial("_TransferConsolidated");
}

<!-- 
IMPLEMENTATION STATUS FOR EACH VIEW
-->

Appointment/Create.cshtml
? Transfer button in header (already exists)
? _TransferConsolidated partial rendered

Schedule/Create.cshtml  
? Transfer button added to header
? _TransferConsolidated partial rendered

Remuneration/Create.cshtml
? Simplified layout
? _TransferConsolidated partial rendered

<!-- 
JAVASCRIPT BEHAVIOR
The button with id="btnOpenTransfer" automatically triggers:
1. Initialize transfer dialog
2. Load available instances
3. Clear previous state
4. Open transfer window

No additional code needed - everything is in _TransferConsolidated.cshtml
-->

<!-- 
CSS CLASSES USED
- k-button: Kendo button styling
- k-button-md: Medium size
- k-rounded-lg: Large border radius
- k-button-outline: Outline style
- k-button-outline-info: Blue color (info theme)
- fa fa-exchange: Font Awesome exchange icon
-->

<!-- STYLING CUSTOMIZATION (if needed) -->

<style>
    /* Adjust button size */
    #btnOpenTransfer.k-button-md {
        padding: 8px 16px;
    }

    /* Adjust header spacing */
    .k-card-header {
        gap: 1rem;
    }

    /* Adjust header layout on small screens */
    @media (max-width: 768px) {
        .k-card-header {
            flex-direction: column;
            align-items: flex-start;
        }

        #btnOpenTransfer {
            width: 100%;
            margin-top: 10px;
        }
    }
</style>

<!-- VERIFICATION CHECKLIST -->

Before deploying, verify:

1. ? Button id="btnOpenTransfer" present in all three Create views
2. ? Button placed in card header with flex layout  
3. ? _TransferConsolidated partial rendered at end of each view
4. ? PaperSettingController is registered in DI
5. ? All three services injected: IAppointmentService, IScheduleService, IRemunerationService
6. ? Anti-forgery token present in form: @Html.AntiForgeryToken()
7. ? CSS classes available (Kendo UI + FontAwesome loaded)
8. ? Build successful with no compiler errors
9. ? Browser DevTools shows no JavaScript errors
10. ? Transfer window opens when button clicked
