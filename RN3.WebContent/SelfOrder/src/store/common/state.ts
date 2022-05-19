import { CommonState } from '../types';

const state: CommonState = {
  patternCD: null,
  tableNo: null,
  businessDate: null,
  encryptWebMemberCD: null,
  encryptRepreAccountNo: null,
  playerList: [] = [],
  multiMode: false,
  menu: null,
  selectedMenu: null,
  targetRequest: [] = [],
  cartItemList: [] = []
};

export default state;
