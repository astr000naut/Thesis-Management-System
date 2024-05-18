
<template>
    <div class="page__container flex-col rg-2">
        <PopupUpload
            v-model:visible="popupUpload.visible"
            pUrlUpload="/worker/api/teachers/upload"
            pUrlDownloadSample="/worker/api/teachers/sample_upload_file"
            @close="popupUploadOnClose"
        ></PopupUpload>
        <div class="page__header flex-row">
            <h1 class="page__title" style="font-size: 24px">
                Danh sách Giảng viên
            </h1>
            <el-button type="primary" @click="btnImportOnClick"
                >Nhập khẩu</el-button
            >
        </div>
        <div class="search-container flex-row al-center cg-2">
            <div class="flex-left">
                <el-select
                v-model="selectedFaculty"
                placeholder="Chọn khoa"
                size="large"
                style="width: 240px"
                @change="facultyOnChanged"
                >
                <el-option
                    v-for="item in facultyOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                />
            </el-select>
            </div>
            <div class="flex-right flex-row cg-2">
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
        </div>
        <div class="table__container fl-1">
            <el-table
                :data="entities"
                style="width: 100%"
                row-key="userId"
                v-loading="loading"
            >
                <el-table-column
                    fixed
                    prop="teacherCode"
                    label="Mã giảng viên"
                    width="250"
                />
                <el-table-column prop="teacherName" label="Tên giảng viên" width="400" />
                <el-table-column prop="facultyName" label="Khoa" width="300" />
                <el-table-column prop="email" label="Email" width="300" />
                <el-table-column
                    prop="phoneNumber"
                    label="Số điện thoại"
                    width="300"
                />
                <el-table-column fixed="right" label="Thao tác">
                    <template #default="scope">
                        <el-button @click="btnDeleteItemOnClick(scope.row)"
                            >Xóa</el-button>
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
                @size-change="teacherStore.setPageSize"
                @current-change="teacherStore.setPageNumber"
            />
        </div>
    </div>
</template>


<script setup>
import { onMounted, ref } from "vue";
import { useTeacherStore, useFacultyStore } from "@/stores";
import { useRouter } from "vue-router";
import { storeToRefs } from "pinia";
import { Refresh, Search } from "@element-plus/icons-vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { debounce } from "lodash";
import PopupUpload from '@/components/common/PopupUpload.vue';

const router = useRouter();
const teacherStore = useTeacherStore();
const facultyStore = useFacultyStore();
const { entities, total, loading, keySearch, pageNumber, pageSize } =
    storeToRefs(teacherStore);
let debouncedFunction = null;

const {entities: facultyEntities} = storeToRefs(facultyStore);

const facultyOptions = ref([
    { value: "-1", label: "Tất cả khoa" },
]);
const selectedFaculty = ref("-1");


const searchText = ref("");
const popupUpload = ref({
    visible: false,
});

initData();

onMounted(() => {
});

async function initData() {
    await teacherStore.fetchList();

    facultyStore.setPageSize(0);
    await facultyStore.fetchList();
    facultyOptions.value = facultyOptions.value.concat(
        facultyEntities.value.map((item) => ({
            value: item.facultyCode,
            label: item.facultyName,
        }))
    );
}

async function facultyOnChanged(value) {
    const customWhere = [
        {
            command: 'AND',
            columnName: 'facultyCode',
            operator: '=',
            value: value
        }
    ];

    if (value === '-1') {
        customWhere.pop();
    }

    await teacherStore.fetchList(customWhere);
}

async function popupUploadOnClose(isUploadSuccess) {
    if (isUploadSuccess) {
        await teacherStore.fetchList();
    }
}

const btnImportOnClick = () => {
    popupUpload.value.visible = true;
};

const btnDeleteItemOnClick = (row) => {
    ElMessageBox.confirm(
        `Bạn có chắc chắn muốn xóa ${row.teacherCode}-${row.teacherName} ?`,
        "Xác nhận",
        {
            confirmButtonText: "Đồng ý",
            cancelButtonText: "Hủy",
            type: "warning",
        }
    ).then(async () => {
        const isDeleted = await teacherStore.deleteOne(row.userId);
        console.log(isDeleted);
        if (isDeleted) {
            ElMessage.success("Xóa thành công");
        } else {
            // ElMessage.error('Xóa thất bại');
        }
    });
};

async function searchTextOnInput() {
    if (!debouncedFunction) {
        debouncedFunction = debounce(() => {
            teacherStore.setKeySearch(searchText.value);
        }, 800);
    }
    debouncedFunction();
}

const btnViewItemOnClick = (row) => {
    router.push(`/tenant/view/${row.tenantId}`);
};

const btnEditItemOnClick = (row) => {
    router.push(`/tenant/edit/${row.tenantId}`);
};

async function btnRefreshOnClick() {
    await teacherStore.fetchList();
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
    justify-content: space-between;
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
