import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
} from '@angular/core';

import locales from '@/locales/parameters';
import { UserParameterService } from '@/shared/services/user-parameter/user-parameter.service';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import {
  ChangeStatusUserToSave,
  UserParameter,
} from '@/shared/models/user-parameter';
import { ActivatedRoute } from '@angular/router';
import { Clipboard } from '@angular/cdk/clipboard';
import { UserService } from '@/shared/services/user/user.service';
import { GenerateLinkAccessFromUserParameter, IPermissions } from '@/shared/models/token';
import { TokenService } from '@/shared/services/token/token.service';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UsersListComponent implements OnInit {
  locales = locales;
  listHeight = 70;
  hasMoreUsers = true;
  page = 1;
  size = 10;
  usersParameter: UserParameter[] = [];
  modalSendLinkAccessData: any;
  permissions: IPermissions;

  constructor(
    private userParameterService: UserParameterService,
    private userService: UserService,
    private ngxSpinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private changeDetectorRef: ChangeDetectorRef,
    private activatedRoute: ActivatedRoute,
    private clipboard: Clipboard,
    private tokenService: TokenService
  ) { }

  ngOnInit() {
    this.permissions = this.tokenService.getPermissions();
    this.usersParameter = this.activatedRoute.snapshot.data.usersParameter;
  }

  async changeStatusUser(userParameter: UserParameter): Promise<void> {
    const changeStatusUserToSave: ChangeStatusUserToSave = {
      userId: userParameter.id,
      active: userParameter.active,
    };

    await this.userParameterService
      .changeStatusUser(changeStatusUserToSave)
      .toPromise();

    this.ngxSpinnerService.hide();
    this.toastrService.success(
      locales.saveSucessMessage,
      locales.saveSucessMessageTitle
    );
  }

  async getAccessLink(userParameter: UserParameter): Promise<void> {
    this.modalSendLinkAccessData = null;
    this.changeDetectorRef.markForCheck();

    const generateLinkAccessFromUserParameter: GenerateLinkAccessFromUserParameter = {
      userId: userParameter.id,
    };

    const accessLink = await this.userService
      .generateLinkAccess(generateLinkAccessFromUserParameter)
      .toPromise();

    this.ngxSpinnerService.hide();

    setTimeout(() => {
      this.clipboard.copy(accessLink);

      this.modalSendLinkAccessData = {
        accessLink,
        userParameter,
      };

      this.changeDetectorRef.markForCheck();
    }, 500);
  }

  async onScroll(): Promise<void> {
    if (this.hasMoreUsers) {
      this.page++;
      this.hasMoreUsers = false;
      this.changeDetectorRef.markForCheck();

      const moreUsers = await this.userParameterService
        .getUserParameters(this.page)
        .toPromise();

      this.listHeight = this.listHeight * 2;
      this.usersParameter.push(...moreUsers);

      setTimeout(() => {
        if (moreUsers.length >= this.size) {
          this.hasMoreUsers = true;
        }

        this.ngxSpinnerService.hide();
        this.changeDetectorRef.markForCheck();
      }, 250);
    }
  }
}
