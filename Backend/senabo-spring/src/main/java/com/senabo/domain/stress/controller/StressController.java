package com.senabo.domain.stress.controller;

import com.senabo.common.api.ApiResponse;
import com.senabo.domain.member.entity.Member;
import com.senabo.domain.member.service.MemberService;
import com.senabo.domain.report.entity.Report;
import com.senabo.domain.report.service.ReportService;
import com.senabo.domain.stress.dto.response.StressResponse;
import com.senabo.domain.stress.entity.Stress;
import com.senabo.domain.stress.entity.StressType;
import com.senabo.domain.stress.service.StressService;
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
@RequestMapping("/stress")
@Tag(name = "Stress", description = "Stress API Document")
public class StressController {
    private final StressService stressService;
    private final MemberService memberService;
    private final ReportService reportService;

    @PostMapping("/save/{type}")
    @Operation(summary = "스트레스 지수 저장", description = "스트레스 지수가 1 상승한다.")
    public ApiResponse<StressResponse> createStress(@AuthenticationPrincipal UserDetails principal, @PathVariable StressType type) {
        StressResponse response = stressService.createStress(principal.getUsername(), type, 1);
        return ApiResponse.success("스트레스 지수 저장 성공", response);
    }

    @GetMapping("/list")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = StressResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "스트레스 지수 전제 조회", description = "스트레스 지수 내역을 전체 조회한다.")
    public ApiResponse<List<StressResponse>> getStress(@AuthenticationPrincipal UserDetails principal) {
        List<Stress> stress = stressService.getStress(principal.getUsername());
        if (stress.isEmpty()) return ApiResponse.fail("스트레스 지수 주간 조회 실패", null);
        List<StressResponse> response = stress.stream().map(StressResponse::from).collect(Collectors.toList());
        return ApiResponse.success("스트레스 지수 전체 조회 성공", response);
    }


    @GetMapping("/list/{week}")
    @ApiResponses(value = {
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "200", description = "내역이 있으면 status: SUCCESS, 내역이 없으면 status: FAIL", content =
                    {@Content(mediaType = "application/json", schema =
                    @Schema(implementation = StressResponse.class))}),
            @io.swagger.v3.oas.annotations.responses.ApiResponse(responseCode = "404", description = "USER NOT FOUND")
    }
    )
    @Operation(summary = "스트레스 지수 주간 조회", description = "스트레스 지수 내역을 주간 조회한다.")
    public ApiResponse<List<StressResponse>> getStress(@AuthenticationPrincipal UserDetails principal, @PathVariable int week) {
        Member member = memberService.findByEmail(principal.getUsername());
        Optional<Report> reportOptional = reportService.findReportWeek(member, week);
        if (reportOptional.isEmpty()) return ApiResponse.fail("스트레스 지수 " + week + "주차 조회 실패", null);
        List<Stress> stress = stressService.getStressWeek(reportOptional.get(), member);
        List<StressResponse> response = stress.stream().map(StressResponse::from).collect(Collectors.toList());
        return ApiResponse.success("스트레스 지수 " + week + "주차 조회 성공", response);
    }
}
