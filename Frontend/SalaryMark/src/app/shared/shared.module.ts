import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FlexLayoutModule } from "@angular/flex-layout";
import { NgxDatatableModule } from "@swimlane/ngx-datatable";

import { NgxSpinnerModule } from "ngx-spinner";
import { ToastrModule } from "ngx-toastr";
import { InfiniteScrollModule } from "ngx-infinite-scroll";

import { BasicDialogStructureComponent } from "./components/basic-dialog-structure/basic-dialog-structure.component";
import { ButtonMoreActionsComponent } from "./components/button-more-actions/button-more-actions.component";
import { TitleHeaderComponent } from "./components/title-header/title-header.component";
import { FilterHeaderComponent } from "./components/filter-header/filter-header.component";
import { SwitchButtonComponent } from "./components/switch-button/switch-button.component";
import { ButtonListComponent } from "./components/button-list/button-list.component";
import { SearchComponent } from "./components/search/search.component";
import { HighlightsPipe } from "./filter/highlights.pipe";
import { FilterTooltipPipe } from "./filter/tooltip.pipe";

import { ExportButtonComponent } from "./components/export-button/export-button.component";
import { TableHeaderModalComponent } from "./components/table-header-modal/table-header-modal.component";
import { ClearFilterComponent } from "./components/clear-filter/clear-filter.component";
import { ModalModule } from "ngx-bootstrap/modal";
import { TooltipModule } from "ngx-bootstrap/tooltip";
import { AccordionModule } from "ngx-bootstrap/accordion";
import { ViewChildShowModalComponent } from "./components/view-child-show-modal/view-child-show-modal.component";
import { ModalShareTableComponent } from "./components/modal-share-table/modal-share-table.component";
import { RouterModule } from "@angular/router";
import { FinancialImpactChartComponent } from "./components/charts/financial-impact-chart/financial-impact-chart.component";
import { HighchartsChartModule } from "highcharts-angular";
import { PositionsDashboardChartComponent } from "./components/charts/positions-dashboard-chart/positions-dashboard-chart.component";
// tslint:disable-next-line: max-line-length
import { ComparativeAnalysisChartComponent } from "./components/charts/comparative-analysis-chart/comparative-analysis-chart.component";
import { DistributionAnalysisChartComponent } from "./components/charts/distribution-analysis-chart/distribution-analysis-chart.component";
import { CheckboxComponent } from "./components/checkbox/checkbox.component";
import { ButtonListLightComponent } from "./components/button-list-light/button-list-light.component";
import { ProposedMovementsChartComponent } from "./components/charts/proposed-movements-chart/proposed-movements-chart.component";
import { ReportItemComponent } from "./components/report-item/report-item.component";
import { ClipboardModule } from "@angular/cdk/clipboard";
import { ShareHeaderComponent } from "./components/share-header/share-header.component";
import { CheckeboxShowDialogComponent } from "./components/checkebox-show-dialog/checkebox-show-dialog.component";
import { ResizeColumnDirective } from "./directives/resize-column.directive";
import { FilterByComponent } from "./components/filter-by/filter-by.component";
import { ComparativeAnalysisChartDashboardComponent } from "./components/charts/comparative-analysis-chart-dashboard/comparative-analysis-chart-dashboard.component";
import { ModalChartComponent } from "./components/charts/modals/modal-chart/modal-chart.component";
import { BsDropdownModule } from "ngx-bootstrap/dropdown";
import { ConfigButtonComponent } from "./components/config-button/config-button.component";
import { RadioButtonComponent } from "./components/lib-forms/radio-button/radio-button.component";
import { ButtonPrimaryComponent } from "./components/lib-forms/button-primary/button-primary.component";
import { ButtonSecundaryComponent } from "./components/lib-forms/button-secundary/button-secundary.component";
import { ErrorMessageComponent } from "./components/error-message/error-message.component";
import { ButtonListVisualizationComponent } from "./components/button-list-visualization/button-list-visualization.component";
import { SliderControllerComponent } from "./components/slider-controller/slider-controller.component";
import { NgxSliderModule } from "@angular-slider/ngx-slider";
import { DropdownSearchComponent } from "./components/dropdown-search/dropdown-search.component";
import { SalarialChartComponent } from "./components/charts/salarial-chart/salarial-chart.component";
import { DelayedInputModule } from "./directives/delayed-input/delayed-input-module.module";
import { ModalAddPositionComponent } from "./components/modal-add-position/modal-add-position.component";
import { TabsModule } from "ngx-bootstrap/tabs";
import { ButtonListLoopLightComponent } from "./components/button-list-loop-light/button-list-loop-light.component";
import { FilterByHeaderComponent } from "./components/filter-header/components/filter-by-header/filter-by-header.component";
import { ModalPositionDetailComponent } from "./components/modal-position-detail/modal-position-detail.component";
import { PositionDetailTabComponent } from "./components/modal-position-detail/common/tabs/position-detail-tab/position-detail-tab.component";
import { SalaryTableTabComponent } from "./components/modal-position-detail/common/tabs/salary-table-tab/salary-table-tab.component";
import { FrameworkTabComponent } from "./components/modal-position-detail/common/tabs/framework-tab/framework-tab.component";
import { LobbuTabComponent } from "./components/modal-position-detail/common/tabs/lobbu-tab/lobbu-tab.component";
import { CareerTrackTabComponent } from "./components/modal-position-detail/common/tabs/career-track-tab/career-track-tab.component";
import { ItemButtonRemoveComponent } from "./components/item-button-remove/item-button-remove.component";
import { DropdownParametersComponent } from "./components/modal-position-detail/common/tabs/components/dropdown-parameters/dropdown-parameters.component";
import { IntensityBarComponent } from "./components/intensity-bar/intensity-bar.component";
import { IntensityPercentComponent } from "./components/intensity-percent/intensity-percent.component";
import { IntensityTableComponent } from "./components/intensity-table/intensity-table.component";
import { DropdownItemTrackerComponent } from "./components/modal-position-detail/common/tabs/career-track-tab/components/dropdown-item-tracker/dropdown-item-tracker.component";
import { BlockedContentComponent } from "./components/blocked-content/blocked-content.component";
import { PublicationDetailComponent } from "./components/publication-detail/publication-detail.component";
import { ConfirmModalEditPositionComponent } from './components/confirm-modal-edit-position/confirm-modal-edit-position.component';
@NgModule({
  declarations: [
    BasicDialogStructureComponent,
    ButtonMoreActionsComponent,
    TitleHeaderComponent,
    FilterHeaderComponent,
    SwitchButtonComponent,
    ButtonListComponent,
    SearchComponent,
    ExportButtonComponent,
    TableHeaderModalComponent,
    ClearFilterComponent,
    HighlightsPipe,
    FilterTooltipPipe,
    ViewChildShowModalComponent,
    ModalShareTableComponent,
    FinancialImpactChartComponent,
    PositionsDashboardChartComponent,
    ComparativeAnalysisChartComponent,
    DistributionAnalysisChartComponent,
    CheckboxComponent,
    ButtonListLightComponent,
    ProposedMovementsChartComponent,
    ReportItemComponent,
    ShareHeaderComponent,
    CheckeboxShowDialogComponent,
    ResizeColumnDirective,
    FilterByComponent,
    ComparativeAnalysisChartDashboardComponent,
    ModalChartComponent,
    ConfigButtonComponent,
    RadioButtonComponent,
    ButtonPrimaryComponent,
    ButtonSecundaryComponent,
    ErrorMessageComponent,
    ButtonListVisualizationComponent,
    SliderControllerComponent,
    DropdownSearchComponent,
    SalarialChartComponent,
    ModalAddPositionComponent,
    ButtonListLoopLightComponent,
    FilterByHeaderComponent,
    ModalPositionDetailComponent,
    PositionDetailTabComponent,
    SalaryTableTabComponent,
    FrameworkTabComponent,
    LobbuTabComponent,
    CareerTrackTabComponent,
    ItemButtonRemoveComponent,
    DropdownParametersComponent,
    IntensityBarComponent,
    IntensityPercentComponent,
    IntensityTableComponent,
    DropdownItemTrackerComponent,
    BlockedContentComponent,
    PublicationDetailComponent,
    ConfirmModalEditPositionComponent,
  ],
  exports: [
    BasicDialogStructureComponent,
    ButtonMoreActionsComponent,
    CommonModule,
    FormsModule,
    FlexLayoutModule,
    TitleHeaderComponent,
    FilterHeaderComponent,
    SwitchButtonComponent,
    ButtonListComponent,
    ToastrModule,
    NgxSpinnerModule,
    ExportButtonComponent,
    TableHeaderModalComponent,
    ClearFilterComponent,
    ModalModule,
    TooltipModule,
    AccordionModule,
    SearchComponent,
    HighlightsPipe,
    FilterTooltipPipe,
    InfiniteScrollModule,
    ViewChildShowModalComponent,
    ModalShareTableComponent,
    RouterModule,
    FinancialImpactChartComponent,
    PositionsDashboardChartComponent,
    ComparativeAnalysisChartComponent,
    DistributionAnalysisChartComponent,
    CheckboxComponent,
    ButtonListLightComponent,
    ProposedMovementsChartComponent,
    ReportItemComponent,
    ClipboardModule,
    ShareHeaderComponent,
    CheckeboxShowDialogComponent,
    NgxDatatableModule,
    ResizeColumnDirective,
    FilterByComponent,
    ComparativeAnalysisChartDashboardComponent,
    ModalChartComponent,
    ConfigButtonComponent,
    RadioButtonComponent,
    ButtonPrimaryComponent,
    ButtonSecundaryComponent,
    ErrorMessageComponent,
    ButtonListVisualizationComponent,
    SliderControllerComponent,
    DropdownSearchComponent,
    SalarialChartComponent,
    ModalAddPositionComponent,
    ButtonListLoopLightComponent,
    ModalPositionDetailComponent,
    IntensityBarComponent,
    IntensityPercentComponent,
    IntensityTableComponent,
    BlockedContentComponent,
  ],
  imports: [
    CommonModule,
    FlexLayoutModule,
    InfiniteScrollModule,
    ToastrModule.forRoot(),
    AccordionModule.forRoot(),
    NgxSpinnerModule,
    ModalModule.forRoot(),
    HighchartsChartModule,
    TooltipModule.forRoot(),
    ClipboardModule,
    NgxDatatableModule,
    BsDropdownModule.forRoot(),
    NgxSliderModule,
    DelayedInputModule,
    FormsModule,
    ReactiveFormsModule,
    TabsModule.forRoot(),
  ],
  providers: [HighlightsPipe, FilterTooltipPipe],
})
export class SharedModule {}
