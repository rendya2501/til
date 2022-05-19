<template>
  <b-container fluid class="header">
    <b-row class="upper half" v-if="!multiMode">
      <b-col class="col-account-info">
        【{{
          context ? context.AccountNo : getHeaderContext.AccountNo
        }}】&nbsp;{{
          context ? context.PlayerName : getHeaderContext.PlayerName
        }}&nbsp;様
      </b-col>
    </b-row>
    <b-row class="lower half" v-if="!multiMode">
      <b-col>
        <label class="lbl-account-total">
          合計金額&nbsp;￥{{
            context
              ? context.TotalPrice.toLocaleString()
              : getHeaderContext.TotalPrice.toLocaleString()
          }}
        </label>
        <label class="lbl-account-numberOfOrders"
          >({{
            context
              ? context.TotalQuantity.toLocaleString()
              : getHeaderContext.TotalQuantity.toLocaleString()
          }}品)</label
        >
      </b-col>
    </b-row>
    <b-row class="full" align-v="center" v-if="multiMode">
      <b-col class="col-amount-info"
        >注文数&nbsp;{{
          context
            ? context.TotalQuantity.toLocaleString()
            : getHeaderContext.TotalQuantity.toLocaleString()
        }}</b-col
      >
      <b-col class="col-total-price-info">
        <label class="lbl-account-total">
          小計&nbsp;{{
            context
              ? context.TotalPrice.toLocaleString()
              : getHeaderContext.TotalPrice.toLocaleString()
          }}円
        </label>
      </b-col>
    </b-row>
  </b-container>
</template>

<script lang="ts">
import { Vue, Component, Prop } from 'vue-property-decorator';
import { HeaderContext } from '@/types/context/HeaderContext';
import { namespace } from 'vuex-class';

const CommonModule = namespace('common');

@Component
export default class Header extends Vue {
  @CommonModule.State
  private multiMode!: boolean;

  @CommonModule.Getter
  private getHeaderContext!: HeaderContext;

  @Prop({ default: null })
  private context!: HeaderContext;
}
</script>

<style scoped>
.header {
  background-color: rgb(50, 147, 223);
  color: white !important;
  height: var(--header-height);
}
.header .full {
  height: 100%;
}
.header .half {
  height: 50%;
  position: relative;
}
.header .title {
  padding-left: 4px;
}

.upper {
  position: absolute;
}

/* お客様氏名 と 会計番号 */
.col-account-info {
  position: absolute;
  text-align: left;
}

.col-amount-info {
  position: absolute;
  text-align: left;
}

.col-total-price-info {
  position: absolute;
  text-align: right;
}

.lower {
  text-align: right;
  white-space: nowrap;
  position: absolute;
}

/* 合計金額 */
.lbl-account-total {
  margin: 0 15px;
}
</style>
