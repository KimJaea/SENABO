package com.senabo.domain.disease.controller;

import com.senabo.common.api.ApiResponse;
import com.senabo.domain.disease.dto.response.DiseaseResponse;
import com.senabo.domain.disease.entity.Disease;
import com.senabo.domain.disease.service.DiseaseService;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Slf4j
@RequiredArgsConstructor
@RestController
@RequestMapping("/disease")
@Tag(name = "Disease", description = "Disease API Document")
public class DiseaseController {

    private final DiseaseService diseaseService;
    private final MemberService memberService;
    private final ReportService reportService;


    @PostMapping("/save/{diseaseName}")
    @Operation(summary = "질병 내역 저장", description = "질병 내역을 저장한다.")
    public ApiResponse<DiseaseResponse> createDisease(@AuthenticationPrincipal UserDetails principal, @PathVariable String diseaseName) {
        DiseaseResponse response = diseaseService.createDisease(principal.getUsername(), diseaseName);
        return ApiResponse.success("질병 저장 완료", response);
    }


    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = DiseaseResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "질병 전체 조회", description = "질병 내역을 전체 조회한다.")
    public ApiResponse<List<DiseaseResponse>> getDisease(@AuthenticationPrincipal UserDetails principal) {
        List<Disease> disease = diseaseService.getDisease(principal.getUsername());
        if (disease.isEmpty()) return ApiResponse.fail("질병 전체 조회 실패", null);
        List<DiseaseResponse> response = disease.stream()
                .map(DiseaseResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("질병 전체 조회 성공", response);
    }


    @GetMapping("/list/{week}")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = DiseaseResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "질병 주간 조회", description = "질병 내역을 주간 조회한다.")
    public ApiResponse<List<DiseaseResponse>> getDiseaseWeek(@AuthenticationPrincipal UserDetails principal, @PathVariable int week) {
        Member member = memberService.findByEmail(principal.getUsername());
        Optional<Report> reportOptional =  reportService.findReportWeek(member, week);
        if(reportOptional.isEmpty()) return ApiResponse.fail("질병 " + week + "주차 조회 실패", null);
        List<Disease> disease = diseaseService.getDiseaseWeek(reportOptional.get(), member);
        List<DiseaseResponse> response = disease.stream()
                .map(DiseaseResponse::from)
                .collect(Collectors.toList());
        return ApiResponse.success("질병 " + week + "주차 조회 성공", response);
    }


    @DeleteMapping("/remove")
    @Operation(summary = "질병 내역 삭제", description = "질병 내역을 삭제한다.")
    public ApiResponse<Object> removeDisease(@AuthenticationPrincipal UserDetails principal) {
        diseaseService.removeDisease(principal.getUsername());
        return ApiResponse.success("질병 삭제 성공", true);
    }
}
