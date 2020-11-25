<?php
// Your code here!
    const DELETE_TYPE_LOGICAL = 3;
    const DELETE_TYPE_PHYSICAL = 1;
    const DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE = 0;
    const DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE = 2;
    
    $delType = DELETE_TYPE_PHYSICAL;
    $landingAchieveFlag = true;
    
    $extendDelType = $delType === DELETE_TYPE_PHYSICAL
        // 物理削除はそのまま
        ? DELETE_TYPE_PHYSICAL
        : $landingAchieveFlag
            // 論理削除+実績あり
            ? DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE
            // 論理削除+実績なし
            : DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE;
            
    $extendDelType2 = (function ($delType, $landingAchieveFlag) {
        if ($delType === DELETE_TYPE_PHYSICAL) {
            return DELETE_TYPE_PHYSICAL;
        } else {
            if ($landingAchieveFlag) {
                return DELETE_TYPE_LOGICAL_WITH_HAS_ACHIEVE;
            } else {
                return DELETE_TYPE_LOGICAL_WITH_NO_ACHIEVE;
            }
        }
    })($delType,$landingAchieveFlag);
    
    var_dump($extendDelType);
    var_dump($extendDelType2);
?>
