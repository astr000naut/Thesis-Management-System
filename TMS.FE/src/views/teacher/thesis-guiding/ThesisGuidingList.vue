<template>
    <div class="page__container flex-col rg-2">
        <ThesisGuidingDetail
            v-model:visible="popupDetail.visible"
            :pEntityId="popupDetail.entityId"
            :pMode="popupDetail.mode"
            @removeItem="onRemoveItem"
        />
        <div class="page__header flex-row">
            <h1 class="page__title" style="font-size: 24px">
                Khóa luận đang hướng dẫn
            </h1>
        </div>
        <div class="search-container flex-row al-center cg-2">
            <div class="reload-btn">
                <el-button :icon="Refresh" circle @click="btnRefreshOnClick" />
            </div>
            <div class="search-input">
                <el-input
                    v-model="searchText"
                    style="width: 240px"
                    placeholder="Tìm kiếm"
                    :prefix-icon="Search"
                    @input="searchTextOnInput"
                    clearable
                />
            </div>
        </div>
        <div class="table__container fl-1">
            <el-table
                :data="entities"
                style="width: 100%"
                :row-key="entityInfo.keyName"
                v-loading="loading"
            >
                <el-table-column
                    fixed
                    prop="thesisCode"
                    label="Mã khóa luận"
                    width="150"
                />
                <el-table-column prop="thesisName" label="Tên đề tài" />
                <el-table-column prop="description" label="Mô tả" width="300" />
                <el-table-column prop="year" label="Năm học" width="90" />
                <el-table-column prop="semester" label="Học kỳ" width="80" />
                <el-table-column
                    prop="studentName"
                    label="Sinh viên"
                    width="200"
                />
                <el-table-column
                    prop="teacherName"
                    label="Cán bộ hướng dẫn"
                    width="200"
                />
                <el-table-column
                    prop="status"
                    label="Trạng thái"
                    width="160"
                    :formatter="thesisStatusFormatter"
                />
                <el-table-column fixed="right" label="Thao tác" width="120">
                    <template #default="scope">
                        <el-dropdown
                            size="small"
                            split-button
                            type="default"
                            @click="btnViewItemOnClick(scope.row)"
                        >
                            Xem
                            <template #dropdown>
                                <el-dropdown-menu>
                                    <el-dropdown-item
                                        @click="btnEditItemOnClick(scope.row)"
                                        >Chỉnh sửa</el-dropdown-item
                                    >
                                    <el-dropdown-item v-if="scope.row.status === ThesisStatusEnum.ApprovedGuiding"
                                        @click="btnApproveTitleOnClick(scope.row)"
                                        >Xác nhận đề tài</el-dropdown-item
                                    >
                                    <el-dropdown-item v-if="scope.row.status === ThesisStatusEnum.ApprovedTitle"
                                        @click="btnCancelApproveTitleOnClick(scope.row)"
                                        >Hủy xác nhận đề tài</el-dropdown-item
                                    >
                                </el-dropdown-menu>
                            </template>
                        </el-dropdown>
                    </template>
                </el-table-column>
            </el-table>
        </div>
        <div class="pagination">
            <el-pagination
                :current-page="pageNumber"
                :page-size="pageSize"
                :page-sizes="[10, 20, 50, 100]"
                :small="false"
                :disabled="loading"
                :background="false"
                layout="total, sizes, prev, pager, next"
                :total="total"
                @size-change="entityStore.setPageSize"
                @current-change="entityStore.setPageNumber"
            />
        </div>
    </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import { useThesisStore, useAuthStore } from "@/stores";
import { useRouter } from "vue-router";
import { storeToRefs } from "pinia";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { debounce } from "lodash";
import ThesisGuidingDetail from "./ThesisGuidingDetail.vue";
import {ThesisStatus, ThesisStatusEnum} from "@/common/enum";

const router = useRouter();
const entityStore = useThesisStore();
const authStore = useAuthStore();

const { entities, total, loading, keySearch, pageNumber, pageSize } =
    storeToRefs(entityStore);

const popupDetail = ref({
    visible: false,
    entityId: null,
    mode: "add",
});

const entityInfo = {
    keyName: "thesisId",
};

function thesisStatusFormatter(row, column, cellValue) {
    return ThesisStatus[cellValue];
}

let debouncedFunction = null;

const searchText = ref("");

initData();

onMounted(() => {
    console.log("onMounted");
});

async function initData() {
    await getThesisRequestList();
}

function onRemoveItem(id) {
    entityStore.removeOneEntity(id);
}


const btnApproveTitleOnClick = (entity) => {
    ElMessageBox.confirm(
        `Xác nhận đề tài ?`,
        "Xác nhận",
        {
            confirmButtonText: "Đồng ý",
            cancelButtonText: "Hủy",
            type: "info",
        }
    ).then(async () => {     
        entity.status = ThesisStatusEnum.ApprovedTitle;
        const result = await entityStore.update(entity);
        if (result) {
            ElMessage.success("Đã xác nhận đề tài!");
        } else {
            // ElMessage.error('Xóa thất bại');
        }
    });
};

const btnCancelApproveTitleOnClick = (entity) => {
    ElMessageBox.confirm(
        `Hủy xác nhận đề tài ?`,
        "Xác nhận",
        {
            confirmButtonText: "Đồng ý",
            cancelButtonText: "Hủy",
            type: "warning",
        }
    ).then(async () => {     
        entity.status = ThesisStatusEnum.ApprovedGuiding;
        const result = await entityStore.update(entity);
        if (result) {
            ElMessage.success("Đã hủy xác nhận đề tài!");
        } else {
            // ElMessage.error('Xóa thất bại');
        }
    });
};


async function searchTextOnInput() {
    if (!debouncedFunction) {
        debouncedFunction = debounce(() => {
            entityStore.setKeySearch(searchText.value);
        }, 800);
    }
    debouncedFunction();
}

const btnViewItemOnClick = (row) => {
    popupDetail.value = {
        visible: true,
        entityId: row[entityInfo.keyName],
        mode: "view",
    };
};

const btnEditItemOnClick = (row) => {
    popupDetail.value = {
        visible: true,
        entityId: row[entityInfo.keyName],
        mode: "edit",
    };
};

async function getThesisRequestList() {
    const teacherId = authStore.loginInfo.user.userId;
    const customWhere = [
        {
            command: 'AND',
            columnName: 'status',
            operator: '!=',
            value: '0'
        },
        {
            command: 'AND',
            columnName: 'status',
            operator: '!=',
            value: '2'
        },
        {
            command: 'AND',
            columnName: 'status',
            operator: '!=',
            value: '4'
        },
        {
            command: 'AND',
            columnName: 'teacherId',
            operator: '=',
            value: teacherId
        }
    ];

    await entityStore.fetchList(customWhere);
}

async function btnRefreshOnClick() {
    await getThesisRequestList();
}
</script>

<style scoped>
.page__container {
    height: 100%;
    position: relative;
}

.el-table {
    color: #000;
    height: 100%;
}

.page__header {
    justify-content: space-between;
    align-items: center;
}

.table__container {
    overflow: auto;
}

.pagination {
    border-radius: 4px;
    display: flex;
    justify-content: flex-end;
    background-color: #fff;
    padding: 4px 10px 4px 10px;
}

:deep(.el-pagination__total) {
    position: absolute;
    left: 10px;
    font-weight: bold;
}

.search-container {
    justify-content: flex-end;
}

:deep(.el-button.is-circle) {
    width: 32px;
    height: 32px !important;
}

:deep(.el-dropdown__caret-button) {
    outline: unset;
}

:deep(.el-dropdown__caret-button:active) {
    border-left: 1px solid #409eff;
}
</style>
