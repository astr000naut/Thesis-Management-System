<template>
    <div class="page__container flex-col rg-2">
        <ThesisSearchDetail
            v-model:visible="popupDetail.visible"
            :pEntityId="popupDetail.entityId"
            :pMode="popupDetail.mode"
        />
        <div class="page__header flex-row">
            <h1 class="page__title" style="font-size: 24px">
                Tra cứu khóa luận tốt nghiệp
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
                        <el-button
                            type="default"
                            @click="btnViewItemOnClick(scope.row)"
                        >
                            Xem
                        </el-button>
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
import { onMounted, ref, computed } from "vue";
import { useThesisStore } from "@/stores";
import { useRouter } from "vue-router";
import { storeToRefs } from "pinia";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { debounce } from "lodash";
import ThesisSearchDetail from "./ThesisSearchDetail.vue";
import {ThesisStatus} from "@/common/enum";

const router = useRouter();
const entityStore = useThesisStore();
const { entities, total, loading, keySearch, pageNumber, pageSize } =
    storeToRefs(entityStore);

const popupDetail = ref({
    visible: false,
    entityId: null,
    mode: "add",
});

const customWhere = computed(() => {
    return [
    {
            command: 'AND',
            columnName: 'status',
            operator: '=',
            value: '4'
        }
    ];
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



async function initData() {
    await getThesisCompletedList();
}


const btnDeleteItemOnClick = (row) => {
    ElMessageBox.confirm(
        `Bạn có chắc chắn muốn xóa khoa ${row.thesisName} ?`,
        "Xác nhận",
        {
            confirmButtonText: "Đồng ý",
            cancelButtonText: "Hủy",
            type: "warning",
        }
    ).then(async () => {
        const isDeleted = await entityStore.deleteOne(row[entityInfo.keyName]);
        console.log(isDeleted);
        if (isDeleted) {
            ElMessage.success("Xóa thành công");
        } else {
            // ElMessage.error('Xóa thất bại');
        }
    });
};

async function getThesisCompletedList() {
    await entityStore.fetchList(customWhere.value);
}

async function searchTextOnInput() {
    if (!debouncedFunction) {
        debouncedFunction = debounce(() => {
            entityStore.setKeySearch(searchText.value, customWhere.value);
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

async function btnRefreshOnClick() {
    await getThesisCompletedList();
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
