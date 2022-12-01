export interface IDialogInput {
  title: string;
  idModal: string;
  disableFooter: boolean;
  fullSize?: boolean;
  btnSecondaryTitle?: string;
  btnPrimaryTitle?: string;
  btnWithoutCancel?: boolean;
  data?: any;
  canRenameColumn?: boolean;
  isInfoPosition?: boolean;
  isRightModal?: boolean;
}
